# SpriteCanvas

<span style="color:red">**Problem:**</span> When a single element changes on the UI Canvas, it dirties the whole Canvas.

The Canvas is the basic component of Unity UI. It generates meshes that represent the UI Elements placed on it, regenerates the meshes when UI Elements change, and issues draw calls to the GPU so that the UI is actually displayed.

...

<span style="color:red">**Solution:**</span> Split up your Canvases.

https://unity.com/how-to/unity-ui-optimization-tips

In a mobile game project, UIs are essential. Pop-ups, events, tournaments, market, leaderboard, purchase screens, etc. In my opinion, opening and closing UIs, scaling visuals, and scattering coins are all challenging to manage, and optimizing Canvas components is difficult.

I wanted to create a simple Sprite Canvas system.

## Installation

1.  **Window** -> **Package Manager** -> **"+"** -> **Add package from git URL**
```
https://github.com/jankatgurbuz/SpriteCanvas.git?path=Assets
```

2. You can also install via git url by adding this entry in your manifest.json
```
"com.jgurbuz.spritecanvas": "https://github.com/jankatgurbuz/SpriteCanvas.git?path=Assets"
```

## Creating a Simple Sprite Canvas System in Unity
Please follow this path: Hierarchy -> Sprite Canvas -> Canvas

**Don't forget to drag and drop the camera**

<img src="Assets/Documentation~/Images/one.png?raw=true" alt="Sprite Canvas" style="margin: 10 0 10px 10px;" />


<img src="Assets/Documentation~/Images/four.png?raw=true" alt="Sprite Canvas" style="margin: 10 0 10px 10px;" />

Each component you create (Image, TextMeshPro, Button) does not have to be a child of the Canvas. There are 3 registration methods:

- Hierarchical registration
- Reference
- Key

<img src="Assets/Documentation~/Images/two.png?raw=true" alt="Sprite Canvas" style="margin: 10 0 10px 10px;" />

To select anchor presets, look at the following image


<img src="Assets/Documentation~/Images/three.png?raw=true" alt="Sprite Canvas" style="margin: 10 0 10px 10px;" />

I am leaving a video below

https://www.youtube.com/watch?v=N0RWDI7bZ1U

Thank you for following this guide!