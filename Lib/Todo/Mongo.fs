module Lib.Todo.Mongo

open MongoDB.Driver
open Microsoft.Extensions.DependencyInjection

let find (collection: IMongoCollection<TodoItem>) (criteria: TodoCriteria): TodoItem [] =
  match criteria with
  | TodoCriteria.All ->
      collection
        .Find(Builders.Filter.Empty)
        .ToEnumerable()
      |> Seq.toArray

let save (collection: IMongoCollection<TodoItem>) (todo: TodoItem): TodoItem =
  let todos =
    collection
      .Find(fun x -> x.Id = todo.Id)
      .ToEnumerable()

  match Seq.isEmpty todos with
  | true -> collection.InsertOne todo
  | false ->
      let filter =
        Builders<TodoItem>
          .Filter.Eq((fun x -> x.Id), todo.Id)

      let update =
        Builders<TodoItem>
          .Update.Set((fun x -> x.Content), todo.Content)
          .Set((fun x -> x.Done), todo.Done)

      collection.UpdateOne(filter, update) |> ignore

  todo

let delete (collection: IMongoCollection<TodoItem>) (id: string): bool =
  collection
    .DeleteOne(Builders<TodoItem>.Filter.Eq((fun x -> x.Id), id))
    .DeletedCount > 0L

type IServiceCollection with
  member this.AddTodoMongoDB(collection : IMongoCollection<TodoItem>) =
    this.AddSingleton<TodoFind>(find collection) |> ignore
    this.AddSingleton<TodoSave>(save collection) |> ignore
    this.AddSingleton<TodoDelete>(delete collection) |> ignore
