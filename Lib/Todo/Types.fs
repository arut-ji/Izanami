namespace Lib.Todo

type TodoItem =
  { Id: string
    Content: string
    Done: bool }

type TodoCriteria = | All

type TodoSave = TodoItem -> TodoItem
type TodoFind = TodoCriteria -> TodoItem []
type TodoDelete = string -> bool
