# EasyDebug

Unity tool aimed at simplifying debugging both in Runtime and Editor.

Find window under Tools/EasyDebug

# Features

- Runtime command line
- Prompts (runtime visual feedback)
- Object serializer
- Pipe console

## Runtime command line

Can be shown or hid by pressing Slash ("/"), when focused, submit using Enter

To add a new custom command, just add a `Command` attribute to your function like this:

```cs
[Command("commandName", ConsoleCommandType.ObjectRelative)]
public void MyFunction(){
    Debug.Log("My function executed!");
}
```

you can also use third argument to set an alias for the object name. For example, if you have commonly used objects, such as manager, player, spawner, scene, etc. and you don't want to type in the whole gameobject name, you can set a static simple alias name for it.

```cs
[Command("dosmth", ConsoleCommandType.ObjectRelative, "player")]
public void MyFunction(){
    ...
}
```

you can also make commands which take arguments of most types like so:

```cs
[Command("add_hp", ConsoleCommandType.ObjectRelative, "player")]
public void MyFunction(int hp){
    ...
}
```

All supported argument types:

int, uint, short, ushort, byte, sbyte, long, bool, string, char, float, double, Vector2, Vector2Int, Vector3, Vector3Int, Vector4 and Quaternion