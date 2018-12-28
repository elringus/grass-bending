## Download package
For Unity 2018.3 and later: [GrassBending.unitypackage](https://github.com/Elringus/GrassBending/releases/download/v0.1-alpha/GrassBending.unitypackage). Check [releases](https://github.com/Elringus/GrassBending/releases) for previous versions support.

Please be aware that you don't need to clone the whole repository in order to use the extension in your project. Either download package from the link above or extract `Assets/GrassBending` folder from the repository project – it contains all the required assets; other stuff is just for testing purposes.

## Description
The package contains a shader used for billboarded terrain details with waving and bending support and components to provide the required bend data to the shader.

![](https://i.gyazo.com/147e29dbee2d98e79d13022eea2f0a66.gif)
![](https://i.gyazo.com/621b7f528ff828c98d270d4c952225a8.gif)

## How to use
Paint grass on the terrain. Make sure the grass is billboarded (it will use a default shader otherwise).

![](https://i.gyazo.com/7838094447e69dc40c2bb39129dc00d1.png)

Add a `BendGrassWhenEnabled` or `BendGrassWhenVisible` component to a gameobject that should apply the bend effect when positioned over the grass.

![](https://i.gyazo.com/d3d0c8ed8afb569e12efccb2970362d2.png)

`BendRadius` controls the radius of the bending sphere with center being the pivot of the gameobject. 

`Priority` is used to control the bending source priority; when concurrent bend sources limit is exceeded, benders with lower priority values will be served first.

You can also add your own implementation of the `IGrassBender` interface instead of using the built-in bender components.

The bending grass shader replaces Unity's default `Hidden/TerrainEngine/Details/BillboardWavingDoublePass` shader used for the terrain grass when `Billboard` option is enabled. It additionally allows to control the wind waving and bending power via the healthy/dry color tint alpha; when alpha of the both colors is zero, the wind and bending won't affect the grass at all.
