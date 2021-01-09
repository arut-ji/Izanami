namespace Lib.Todo

open Giraffe
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open System

module _Handlers =
  let findAll: HttpFunc -> HttpContext -> HttpFuncResult =
    GET
    >=> route "/todos"
    >=> fun next ctx ->
          let find = ctx.GetService<TodoFind>()
          let todos = find TodoCriteria.All
          json todos next ctx

  let createOne: HttpFunc -> HttpContext -> HttpFuncResult =
    POST
    >=> route "/todos"
    >=> fun next ctx ->
          task {
            let save = ctx.GetService<TodoSave>()
            let! todo = ctx.BindJsonAsync<TodoItem>()

            let todo =
              { todo with
                  Id = ShortGuid.fromGuid (Guid.NewGuid()) }

            return! json (save todo) next ctx
          }

  let updateOne: HttpFunc -> HttpContext -> HttpFuncResult =
    PUT
    >=> routef "/todos/%s" (fun id next ctx ->
          task {
            let save = ctx.GetService<TodoSave>()
            let! todo = ctx.BindJsonAsync<TodoItem>()
            let todo = { todo with Id = id }
            return! json (save todo) next ctx
          })

  let deleteOne: HttpFunc -> HttpContext -> HttpFuncResult =
    DELETE
    >=> routef "/todos/%s" (fun id next ctx ->
          let delete = ctx.GetService<TodoDelete>()
          json (delete id) next ctx)


module Routes =
  let handlers: HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ _Handlers.findAll
             _Handlers.createOne
             _Handlers.updateOne
             _Handlers.deleteOne ]
