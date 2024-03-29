tool
extends Button
class_name ControllerButton

export(String) var path : String = "" setget set_path
export(int, "Both", "Keyboard/Mouse", "Controller") var show_only : int = 0 setget set_show_only
export(int, "None", "Keyboard/Mouse", "Controller") var force_type : int = 0 setget set_force_type

func _ready():
    ControllerIcons.connect("input_type_changed", self, "_on_input_type_changed")
    set_path(path)

func _on_input_type_changed(input_type):
    if show_only == 0 or \
        (show_only == 1 and input_type == ControllerIcons.InputType.KEYBOARD_MOUSE) or \
        (show_only == 2 and input_type == ControllerIcons.InputType.CONTROLLER):
        set_path(path)
    else:
        icon = null

func set_path(_path: String):
    path = _path
    if is_inside_tree():
        if force_type > 0:
            icon = ControllerIcons.parse_path(path, force_type - 1)
        else:
            icon = ControllerIcons.parse_path(path)

func set_show_only(_show_only: int):
    show_only = _show_only
    _on_input_type_changed(ControllerIcons._last_input_type)

func set_force_type(_force_type: int):
    force_type = _force_type
    _on_input_type_changed(ControllerIcons._last_input_type)
