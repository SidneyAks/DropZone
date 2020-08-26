### What is Dropzone?

Dropzone is a customizable system built in order to expand windows snapping functionality. Users can customize individual areas in which to snap their applications to using a keystroke and mouse combination.

![View of Dropzone functionality](Dropzone.gif)

### How does it work?

The Dropzone app runs in the background, monitoring your keystrokes. When you hit the trigger keys (ctrl + alt + middle click) it activates, displaying a user defined list of zones in which you can drop a given window to. This allowes for more customizable snapping behavior. Additionally, differing layouts can be activated and cycled through as shown above.

### Can I download dropzone as an executable without having to compile it myself?

Not in any way that is supported by the author. This application watches your keyboard and mouse input across the entire operating system -- such behavior could easily be abused by a malicious compiler, as such I strongly recommend you download the code and compile it yourself (You can compile it using visual studio community edition 2019 or later). 

### How do I customize the zones?

Currently zones are customizable by modifying the config file (there are plans to update this to a nice GUI). The zones are described by a "Target" and "Trigger". A trigger defines the bounds in which you can drop a window, a target defines the where the window goes -- As you can see in the gif above, it's possible to have different target and trigger bounds. The config file, located at %localappdata%\DropZone\ can be modified to add new layouts and new zones. As you can see below, four zones are defined, with differing targets and triggers. Additionally, they have the "layout" paremeter, which can be either "PerScreen" or "Spanning" (for when a user has multiple monitors and wants a zone to span multiple monitors)

```
<Layout Name="Vertical Thirds">
    <Zones>
        <DropZone Name="Top 1/3" Layout="PerScreen">
            <Target>
                <Left>0</Left>
                <Top>0</Top>
                <Right>1</Right>
                <Bottom>1/3</Bottom>
            </Target>
            <Trigger>
                <Left>0</Left>
                <Top>0</Top>
                <Right>1</Right>
                <Bottom>1/3</Bottom>
            </Trigger>
        </DropZone>
        <DropZone Name="Top 2/3" Layout="PerScreen">
            <Target>
                <Left>0</Left>
                <Top>0</Top>
                <Right>1</Right>
                <Bottom>2/3</Bottom>
            </Target>
            <Trigger>
                <Left>0</Left>
                <Top>1/3</Top>
                <Right>1</Right>
                <Bottom>1/2</Bottom>
            </Trigger>
        </DropZone>
        <DropZone Name="Bottom 2/3" Layout="PerScreen">
            <Target>
                <Left>0</Left>
                <Top>1/3</Top>
                <Right>1</Right>
                <Bottom>1</Bottom>
            </Target>
            <Trigger>
                <Left>0</Left>
                <Top>1/2</Top>
                <Right>1</Right>
                <Bottom>2/3</Bottom>
            </Trigger>
        </DropZone>
        <DropZone Name="Bottom 1/3" Layout="PerScreen">
            <Target>
                <Left>0</Left>
                <Top>2/3</Top>
                <Right>1</Right>
                <Bottom>1</Bottom>
            </Target>
            <Trigger>
                <Left>0</Left>
                <Top>2/3</Top>
                <Right>1</Right>
                <Bottom>1</Bottom>
            </Trigger>
        </DropZone>
    </Zones>
</Layout>
```