# About
**Spline Bones** is a tool for Unity that helps with tasks related to spline deformation.
The idea behind it is very simple. It allows you to create GameObject points (referred to as bones) and assign their influence to individual spline points.
As a result, you can modify the shape of the spline using a set of GameObjects, enabling:
* Animations
* Physics
> And anything else you can come up with within the limits of what GameObjects allow!

Currently, this tool only supports working with **UnityEngine.U2D.Spline**
In the future, an interface for supporting other spline implementations may be added.

I have prepared a [small set of usage examples](https://github.com/kulikIlyaG/ikg-spline-bones-samples) I hope they will help you learn to use the tool more quickly.

>Why keep it a secret?
I’ve been thinking about possibly adding support for 3D mesh deformation.
However, I believe that would be part of a separate tool.

## How To Use?
1. First, you need to create a ScriptableObject that will store information about your object's skeleton.
Right-click in the Project window and navigate to:
```Create -> IKGTools -> Spline Bones -> Skeleton```

2. Next, you need one more ScriptableObject (and this is the last one! :)).
It will store information about which bones influence the spline points and how strongly.
Right-click in the Project window and navigate to:
```Create -> IKGTools -> Spline Bones -> Binding Data```

3. Assign the previously created Skeleton to Binding Data.

4. Now you can create an object called RiggedSpline.
Right-click in the Hierarchy window and select ```RiggedSpline```.

5. Assign the **Binding Data** reference to the newly created object.

Now your **RiggedSpline** is ready for configuration!

And now the inspector of your **RiggedSpline** should look something like this:

![empty-rigged-spline](https://github.com/user-attachments/assets/44f2c5a6-5b96-42e3-a947-222e95e7ff23)

In this inspector, we can see three buttons—well, actually, two buttons and one toggle.

Let’s go over them, even though they’re quite simple:
* **StartExecute/StopExecute** – *This button enables or disables the deformation of your spline.*
* **AddPoint** – *Adds a new point to your BindingsData that corresponds to a point on your spline.
For example, if your spline has 4 points, you need to add 4 points here as well.*
* **RemovePoint** – *If you accidentally added too many points by pressing the previous button too often, this button will help you remove the extra ones. :)*

Since the default spline created by the Sprite Shape Controller contains 4 points,
let’s add these 4 points to our BindingsData.

After that, you should see a result similar to this:

![added-points](https://github.com/user-attachments/assets/f10a7ed8-2075-4555-872b-5820945d0360)

But since the skeleton data is empty. Looking inside the point, you will see that it is empty. Therefore, let's move on to creating bones.

### How Create Bones?
When you select your RiggedSpline or its child object, a small control panel appears in the scene.
>If you suddenly can't see it, it's most likely hidden in the top left corner of the scene :)

To create bones, you just need to switch to skeleton editing mode.

![edit-mode](https://github.com/user-attachments/assets/036910a8-ebae-444a-adfd-603fdb390f4e)

And then choose the bone creation mode.

![add-bone](https://github.com/user-attachments/assets/06a8d472-9cd3-4584-9e7e-cbab3ec226a1)

Then click in the scene where you want your bone to be.

Let's create 4 bones approximately where each spline point is located.
After that, in the inspector, we will be able to adjust the influence strength of each bone to each point. It should look something like this:
![weight-edit-example](https://github.com/user-attachments/assets/17946a01-a5fe-4ecb-8063-5010dcbb4993)
>A double-click on the bone slider will either assign it a value of 0% or 100%

A bit more about the control panel in the scene window
There are 4 buttons. And although you are already familiar with 2 of them, there's still something to talk about.
In order:

* Spline editing button ![spline-edit-button](https://github.com/user-attachments/assets/52d5fea1-1048-4b5d-9bad-553e9c17d349)


* Skeleton editing button ![skeleton-edit-button](https://github.com/user-attachments/assets/79a1981e-f39d-4d8b-85df-b48a09a31c33)

These two buttons are designed to give you the ability to modify either the spline or the skeleton.
When you activate one of them and at the same time have spline deformation enabled, they will disable it so that you can change the positions of points or bones.
After disabling them, they will turn the deformation back on, and also recalculate the offsets of points from bones.

---

### API
At the moment, there is no API :)
The current state of the tool is far from ideal.
And now it only allows configuring it as described above and using it within these limits.
Also, changing the RiggedSpline, namely the skeleton or the spline itself (adding or removing points/bones) is not possible at runtime :)

---

### Installation
To install, open Package Manager
and click ```+ -> Add package from git URL...```
and specify this link: 

```https://github.com/kulikIlyaG/ikg-spline-bones.git```

Definitely supported by editor version 2022.3+

>Although it may work with earlier versions :)

List of **dependencies** at the moment:

* com.unity.2d.spriteshape 8.0.0+

---

  ### Road map :)
It would be great to implement in the future:

* Optimize spline deformation calculations (JobsSystem)
* Add support for working with a custom spline implementation
* Bug fixes (there are definitely some, at least on the editor side)
* Add more spline-related functions (primitive shape generator, spline import from another source, etc.)
