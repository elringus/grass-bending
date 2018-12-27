//#define DEBUG_CC2D_RAYS

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityCommon
{

    public class CharacterCollisionState2D
    {
        public bool Right, Left, Above, Below;
        public bool BecameGroundedThisFrame, WasGroundedLastFrame;
        public bool MovingDownSlope;
        public float SlopeAngle;

        public bool HasCollision () => Below || Right || Left || Above;

        public void Reset ()
        {
            Right = Left = Above = Below = BecameGroundedThisFrame = MovingDownSlope = false;
            SlopeAngle = 0f;
        }

        public override string ToString ()
        {
            return string.Format("[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}, movingDownSlope: {4}, angle: {5}, wasGroundedLastFrame: {6}, becameGroundedThisFrame: {7}",
                                 Right, Left, Above, Below, MovingDownSlope, SlopeAngle, WasGroundedLastFrame, BecameGroundedThisFrame);
        }
    }

    [RequireComponent(typeof(BoxCollider2D)), DisallowMultipleComponent]
    public class CharacterController2D : MonoBehaviour
    {
        public enum MoveDirection2D { Idle, Left, Right }

        private struct CharacterRaycastOrigins { public Vector3 TopLeft, BottomRight, BottomLeft; }

        private const float SKIN_WIDTH_FUDGE = 0.001f;

        public event Action<RaycastHit2D> OnControllerCollided;
        public event Action<Collider2D> OnTriggerEnter;
        public event Action<Collider2D> OnTriggerStay;
        public event Action<Collider2D> OnTriggerExit;

        public event Action OnJumped;
        public event Action OnLanded;
        public event Action OnStartedMoving;
        public event Action OnStoppedMoving;
        public event Action<MoveDirection2D> OnMoveDirectionChanged;

        /// <summary>
        /// Defines how far in from the edges of the collider rays are cast from. If cast with a 0 extent it will often result in ray hits that are
        /// not desired (for example a foot collider casting horizontally from directly on the surface can result in a hit).
        /// </summary>
        public float SkinWidth
        {
            get { return skinWidth; }
            set
            {
                skinWidth = value;
                RecalculateDistanceBetweenRays();
            }
        }
        public bool IsMoving => Velocity.sqrMagnitude > 1f;
        public bool IsGrounded => collisionState.Below;
        public Vector3 Velocity => velocity;
        public MoveDirection2D MoveDirection => velocity.x > 0 ? MoveDirection2D.Right : velocity.x < 0 ? MoveDirection2D.Left : MoveDirection2D.Idle;

        [Header("Collision")]
        [Tooltip("Mask with all layers that the player should interact with.")]
        public LayerMask PlatformMask = ~0;
        [Tooltip("Mask with all layers that should act as one-way platforms.")]
        public LayerMask OneWayPlatformMask = 0;
        [Tooltip("Mask with all layers that trigger events should fire when intersected.")]
        public LayerMask TriggerMask = ~0;
        [Range(2, 20)]
        public int TotalHorizontalRays = 8;
        [Range(2, 20)]
        public int TotalVerticalRays = 4;

        [Header("Movement")]
        [Range(0f, 90f), Tooltip("The max slope angle character can climb.")]
        public float SlopeLimit = 30f;
        [Tooltip("The threshold in the change in vertical movement between frames that constitutes jumping.")]
        public float JumpingThreshold = 0.07f;
        [Tooltip("Curve for multiplying speed based on slope (negative = down slope and positive = up slope).")]
        public AnimationCurve SlopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90f, 1.5f), new Keyframe(0f, 1f), new Keyframe(90f, 0f));
        public float Gravity = -25f;
        public float RunSpeed = 8f;
        [Tooltip("How fast character changes direction. Higher means faster.")]
        public float GroundDamping = 20f;
        public float InAirDamping = 5f;
        public float JumpHeight = 3f;

        [Header("Input")]
        public string HorizontalAxisName = "Horizontal";
        public string VerticalAxisName = "Vertical";
        public string JumpButtonName = "Jump";

        private new Transform transform;
        private BoxCollider2D boxCollider;
        private CharacterCollisionState2D collisionState = new CharacterCollisionState2D();
        private Vector3 velocity;
        private Vector3 inputVelocity;
        private bool ignoreOneWayPlatformsThisFrame;
        private float normalizedHorizontalSpeed;
        private float skinWidth = 0.02f;
        /// <summary>
        /// This is used to calculate the downward ray that is cast to check for slopes. We use the somewhat arbitrary value 75 degrees
        /// to calculate the length of the ray that checks for slopes.
        /// </summary>
        private float slopeLimitTangent = Mathf.Tan(75f * Mathf.Deg2Rad);
        /// <summary>
        /// Holder for our raycast origin corners (TR, TL, BR, BL).
        /// </summary>
        private CharacterRaycastOrigins raycastOrigins;
        /// <summary>
        /// Stores our raycast hit during movement.
        /// </summary>
        private RaycastHit2D raycastHit;
        /// <summary>
        /// Stores any raycast hits that occur this frame. we have to store them in case we get a hit moving
        /// horizontally and vertically so that we can send the events after all collision state is set.
        /// </summary>
        private List<RaycastHit2D> raycastHitsThisFrame = new List<RaycastHit2D>(2);
        private float verticalDistanceBetweenRays;
        private float horizontalDistanceBetweenRays;
        /// <summary>
        /// Flag used to mark the case where we are travelling up a slope and we modified our delta.y to allow the climb to occur.
        /// The reason is that if we reach the end of the slope we can make an adjustment to stay grounded.
        /// </summary>
        private bool isGoingUpSlope = false;
        private bool wasGroundedLastFrame;
        private MoveDirection2D moveDirectionLastFrame;
        private bool wasMovingLastFrame;

        #region Monobehaviour

        private void Awake ()
        {
            // Add our one-way platforms to our normal platform mask so that we can land on them from above.
            PlatformMask |= OneWayPlatformMask;

            // Cache components.
            transform = GetComponent<Transform>();
            boxCollider = GetComponent<BoxCollider2D>();

            // Here, we trigger our properties that have setters with bodies.
            SkinWidth = skinWidth;

            // We want to set our CC2D to ignore all collision layers except what is in our triggerMask.
            for (var i = 0; i < 32; i++)
            {
                // See if our triggerMask contains this layer and if not ignore it.
                if ((TriggerMask.value & 1 << i) == 0)
                    Physics2D.IgnoreLayerCollision(gameObject.layer, i);
            }
        }

        private void Update ()
        {
            HandleInput();
            DetectLanding();
            DetectMovement();
            DetectMoveDirection();
        }

        private void OnTriggerEnter2D (Collider2D col)
        {
            OnTriggerEnter?.Invoke(col);
        }


        private void OnTriggerStay2D (Collider2D col)
        {
            OnTriggerStay?.Invoke(col);
        }


        private void OnTriggerExit2D (Collider2D col)
        {
            OnTriggerExit?.Invoke(col);
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
        /// stop when run into.
        /// </summary>
        /// <param name="deltaMovement">Delta movement.</param>
        public void Move (Vector3 deltaMovement)
        {
            // save off our current grounded state which we will use for wasGroundedLastFrame and becameGroundedThisFrame
            collisionState.WasGroundedLastFrame = collisionState.Below;

            // clear our state
            collisionState.Reset();
            raycastHitsThisFrame.Clear();
            isGoingUpSlope = false;

            PrimeRaycastOrigins();

            // first, we check for a slope below us before moving
            // only check slopes if we are going down and grounded
            if (deltaMovement.y < 0f && collisionState.WasGroundedLastFrame)
                HandleVerticalSlope(ref deltaMovement);

            // now we check movement in the horizontal dir
            if (deltaMovement.x != 0f)
                MoveHorizontally(ref deltaMovement);

            // next, check movement in the vertical dir
            if (deltaMovement.y != 0f)
                MoveVertically(ref deltaMovement);

            // move then update our state
            deltaMovement.z = 0;
            transform.Translate(deltaMovement, Space.World);

            // only calculate velocity if we have a non-zero deltaTime
            if (Time.deltaTime > 0f)
                velocity = deltaMovement / Time.deltaTime;

            // set our becameGrounded state based on the previous and current collision state
            if (!collisionState.WasGroundedLastFrame && collisionState.Below)
                collisionState.BecameGroundedThisFrame = true;

            // if we are going up a slope we artificially set a y velocity so we need to zero it out here
            if (isGoingUpSlope)
                velocity.y = 0;

            // send off the collision events if we have a listener
            if (OnControllerCollided != null)
            {
                for (var i = 0; i < raycastHitsThisFrame.Count; i++)
                    OnControllerCollided.Invoke(raycastHitsThisFrame[i]);
            }

            ignoreOneWayPlatformsThisFrame = false;
        }

        /// <summary>
        /// Moves directly down until grounded.
        /// </summary>
        public void WarpToGrounded ()
        {
            do
            {
                Move(new Vector3(0, -1f, 0));
            } while (!IsGrounded);
        }

        /// <summary>
        /// This should be called anytime you have to modify the BoxCollider2D at runtime. It will recalculate the distance between the rays used for collision detection.
        /// It is also used in the skinWidth setter in case it is changed at runtime.
        /// </summary>
        public void RecalculateDistanceBetweenRays ()
        {
            // figure out the distance between our rays in both directions
            // horizontal
            var colliderUseableHeight = boxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2f * skinWidth);
            verticalDistanceBetweenRays = colliderUseableHeight / (TotalHorizontalRays - 1);

            // vertical
            var colliderUseableWidth = boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2f * skinWidth);
            horizontalDistanceBetweenRays = colliderUseableWidth / (TotalVerticalRays - 1);
        }

        #endregion

        #region Movement

        /// <summary>
        /// resets the raycastOrigins to the current extents of the box collider inset by the skinWidth. It is inset
        /// to avoid casting a ray from a position directly touching another collider which results in wonky normal data.
        /// </summary>
        private void PrimeRaycastOrigins ()
        {
            // our raycasts need to be fired from the bounds inset by the skinWidth
            var modifiedBounds = boxCollider.bounds;
            modifiedBounds.Expand(-2f * skinWidth);

            raycastOrigins.TopLeft = new Vector2(modifiedBounds.min.x, modifiedBounds.max.y);
            raycastOrigins.BottomRight = new Vector2(modifiedBounds.max.x, modifiedBounds.min.y);
            raycastOrigins.BottomLeft = modifiedBounds.min;
        }


        /// <summary>
        /// we have to use a bit of trickery in this one. The rays must be cast from a small distance inside of our
        /// collider (skinWidth) to avoid zero distance rays which will get the wrong normal. Because of this small offset
        /// we have to increase the ray distance skinWidth then remember to remove skinWidth from deltaMovement before
        /// actually moving the player
        /// </summary>
        private void MoveHorizontally (ref Vector3 deltaMovement)
        {
            var isGoingRight = deltaMovement.x > 0;
            var rayDistance = Mathf.Abs(deltaMovement.x) + skinWidth;
            var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
            var initialRayOrigin = isGoingRight ? raycastOrigins.BottomRight : raycastOrigins.BottomLeft;

            for (var i = 0; i < TotalHorizontalRays; i++)
            {
                var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * verticalDistanceBetweenRays);

                DrawRay(ray, rayDirection * rayDistance, Color.red);

                // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
                // walk up sloped oneWayPlatforms
                if (i == 0 && collisionState.WasGroundedLastFrame)
                    raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, PlatformMask);
                else raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, PlatformMask & ~OneWayPlatformMask);

                if (raycastHit)
                {
                    // the bottom ray can hit a slope but no other ray can so we have special handling for these cases
                    if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(raycastHit.normal, Vector2.up)))
                    {
                        raycastHitsThisFrame.Add(raycastHit);
                        break;
                    }

                    // set our new deltaMovement and recalculate the rayDistance taking it into account
                    deltaMovement.x = raycastHit.point.x - ray.x;
                    rayDistance = Mathf.Abs(deltaMovement.x);

                    // remember to remove the skinWidth from our deltaMovement
                    if (isGoingRight)
                    {
                        deltaMovement.x -= skinWidth;
                        collisionState.Right = true;
                    }
                    else
                    {
                        deltaMovement.x += skinWidth;
                        collisionState.Left = true;
                    }

                    raycastHitsThisFrame.Add(raycastHit);

                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    if (rayDistance < skinWidth + SKIN_WIDTH_FUDGE)
                        break;
                }
            }
        }

        /// <summary>
        /// Handles adjusting deltaMovement if we are going up a slope.
        /// </summary>
        /// <returns><c>true</c>, if horizontal slope was handled, <c>false</c> otherwise.</returns>
        /// <param name="deltaMovement">Delta movement.</param>
        /// <param name="angle">Angle.</param>
        private bool HandleHorizontalSlope (ref Vector3 deltaMovement, float angle)
        {
            // disregard 90 degree angles (walls)
            if (Mathf.RoundToInt(angle) == 90)
                return false;

            // if we can walk on slopes and our angle is small enough we need to move up
            if (angle < SlopeLimit)
            {
                // we only need to adjust the deltaMovement if we are not jumping
                // TODO: this uses a magic number which isn't ideal! The alternative is to have the user pass in if there is a jump this frame
                if (deltaMovement.y < JumpingThreshold)
                {
                    // apply the slopeModifier to slow our movement up the slope
                    var slopeModifier = SlopeSpeedMultiplier.Evaluate(angle);
                    deltaMovement.x *= slopeModifier;

                    // we dont set collisions on the sides for this since a slope is not technically a side collision.
                    // smooth y movement when we climb. we make the y movement equivalent to the actual y location that corresponds
                    // to our new x location using our good friend Pythagoras
                    deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
                    var isGoingRight = deltaMovement.x > 0;

                    // safety check. we fire a ray in the direction of movement just in case the diagonal we calculated above ends up
                    // going through a wall. if the ray hits, we back off the horizontal movement to stay in bounds.
                    var ray = isGoingRight ? raycastOrigins.BottomRight : raycastOrigins.BottomLeft;
                    RaycastHit2D raycastHit;
                    if (collisionState.WasGroundedLastFrame)
                        raycastHit = Physics2D.Raycast(ray, deltaMovement.normalized, deltaMovement.magnitude, PlatformMask);
                    else
                        raycastHit = Physics2D.Raycast(ray, deltaMovement.normalized, deltaMovement.magnitude, PlatformMask & ~OneWayPlatformMask);

                    if (raycastHit)
                    {
                        // we crossed an edge when using Pythagoras calculation, so we set the actual delta movement to the ray hit location
                        deltaMovement = (Vector3)raycastHit.point - ray;
                        if (isGoingRight)
                            deltaMovement.x -= skinWidth;
                        else
                            deltaMovement.x += skinWidth;
                    }

                    isGoingUpSlope = true;
                    collisionState.Below = true;
                }
            }
            else // too steep. get out of here
            {
                deltaMovement.x = 0;
            }

            return true;
        }


        private void MoveVertically (ref Vector3 deltaMovement)
        {
            var isGoingUp = deltaMovement.y > 0;
            var rayDistance = Mathf.Abs(deltaMovement.y) + skinWidth;
            var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
            var initialRayOrigin = isGoingUp ? raycastOrigins.TopLeft : raycastOrigins.BottomLeft;

            // apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
            initialRayOrigin.x += deltaMovement.x;

            // if we are moving up, we should ignore the layers in oneWayPlatformMask
            var mask = PlatformMask;
            if ((isGoingUp && !collisionState.WasGroundedLastFrame) || ignoreOneWayPlatformsThisFrame)
                mask &= ~OneWayPlatformMask;

            for (var i = 0; i < TotalVerticalRays; i++)
            {
                var ray = new Vector2(initialRayOrigin.x + i * horizontalDistanceBetweenRays, initialRayOrigin.y);

                DrawRay(ray, rayDirection * rayDistance, Color.red);
                raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, mask);
                if (raycastHit)
                {
                    // set our new deltaMovement and recalculate the rayDistance taking it into account
                    deltaMovement.y = raycastHit.point.y - ray.y;
                    rayDistance = Mathf.Abs(deltaMovement.y);

                    // remember to remove the skinWidth from our deltaMovement
                    if (isGoingUp)
                    {
                        deltaMovement.y -= skinWidth;
                        collisionState.Above = true;
                    }
                    else
                    {
                        deltaMovement.y += skinWidth;
                        collisionState.Below = true;
                    }

                    raycastHitsThisFrame.Add(raycastHit);

                    // this is a hack to deal with the top of slopes. if we walk up a slope and reach the apex we can get in a situation
                    // where our ray gets a hit that is less then skinWidth causing us to be ungrounded the next frame due to residual velocity.
                    if (!isGoingUp && deltaMovement.y > 0.00001f)
                        isGoingUpSlope = true;

                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    if (rayDistance < skinWidth + SKIN_WIDTH_FUDGE)
                        break;
                }
            }
        }

        /// <summary>
        /// Checks the center point under the BoxCollider2D for a slope. If it finds one then the deltaMovement is adjusted so that
        /// the player stays grounded and the slopeSpeedModifier is taken into account to speed up movement.
        /// </summary>
        /// <param name="deltaMovement">Delta movement.</param>
        private void HandleVerticalSlope (ref Vector3 deltaMovement)
        {
            // slope check from the center of our collider
            var centerOfCollider = (raycastOrigins.BottomLeft.x + raycastOrigins.BottomRight.x) * 0.5f;
            var rayDirection = -Vector2.up;

            // the ray distance is based on our slopeLimit
            var slopeCheckRayDistance = slopeLimitTangent * (raycastOrigins.BottomRight.x - centerOfCollider);

            var slopeRay = new Vector2(centerOfCollider, raycastOrigins.BottomLeft.y);
            DrawRay(slopeRay, rayDirection * slopeCheckRayDistance, Color.yellow);
            raycastHit = Physics2D.Raycast(slopeRay, rayDirection, slopeCheckRayDistance, PlatformMask);
            if (raycastHit)
            {
                // bail out if we have no slope
                var angle = Vector2.Angle(raycastHit.normal, Vector2.up);
                if (angle == 0)
                    return;

                // we are moving down the slope if our normal and movement direction are in the same x direction
                var isMovingDownSlope = Mathf.Sign(raycastHit.normal.x) == Mathf.Sign(deltaMovement.x);
                if (isMovingDownSlope)
                {
                    // going down we want to speed up in most cases so the slopeSpeedMultiplier curve should be > 1 for negative angles
                    var slopeModifier = SlopeSpeedMultiplier.Evaluate(-angle);
                    // we add the extra downward movement here to ensure we "stick" to the surface below
                    deltaMovement.y += raycastHit.point.y - slopeRay.y - SkinWidth;
                    deltaMovement.x *= slopeModifier;
                    collisionState.MovingDownSlope = true;
                    collisionState.SlopeAngle = angle;
                }
            }
        }

        #endregion

        #region Input
        private void HandleInput ()
        {
            if (IsGrounded)
                inputVelocity.y = 0;

            normalizedHorizontalSpeed = Input.GetAxis(HorizontalAxisName);
            var vertiacalAxis = Input.GetAxis(VerticalAxisName);

            // We can only jump whilst grounded.
            if (IsGrounded && Input.GetButtonDown(JumpButtonName))
            {
                inputVelocity.y = Mathf.Sqrt(2f * JumpHeight * -Gravity);
                OnJumped?.Invoke();
            }

            // Apply horizontal speed smoothing it. 
            var smoothedMovementFactor = IsGrounded ? GroundDamping : InAirDamping; // how fast do we change direction?
            inputVelocity.x = Mathf.Lerp(inputVelocity.x, normalizedHorizontalSpeed * RunSpeed, Time.deltaTime * smoothedMovementFactor);

            // Apply gravity before moving.
            inputVelocity.y += Gravity * Time.deltaTime;

            // If holding down bump up our movement amount and turn off one way platform detection for a frame.
            // This lets us jump down through one way platforms.
            if (IsGrounded && vertiacalAxis < 0)
            {
                inputVelocity.y *= 3f;
                ignoreOneWayPlatformsThisFrame = true;
            }

            Move(inputVelocity * Time.deltaTime);

            // Grab our current velocity to use as a base for all calculations.
            inputVelocity = Velocity;
        }

        private void DetectLanding ()
        {
            if (IsGrounded && !wasGroundedLastFrame)
                OnLanded?.Invoke();
            wasGroundedLastFrame = IsGrounded;
        }

        private void DetectMoveDirection ()
        {
            if (moveDirectionLastFrame != MoveDirection)
                OnMoveDirectionChanged?.Invoke(MoveDirection);
            moveDirectionLastFrame = MoveDirection;
        }

        private void DetectMovement ()
        {
            if (!wasMovingLastFrame && IsMoving)
                OnStartedMoving?.Invoke();
            if (wasMovingLastFrame && !IsMoving)
                OnStoppedMoving?.Invoke();
            wasMovingLastFrame = IsMoving;
        }
        #endregion

        [System.Diagnostics.Conditional("DEBUG_CC2D_RAYS")]
        private void DrawRay (Vector3 start, Vector3 dir, Color color)
        {
            Debug.DrawRay(start, dir, color);
        }
    }
}
