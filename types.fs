namespace metatp

open System

type MetaColumn = {name: string; coltype: Type}
type MetaTable = {name: string; columns: MetaColumn[]}
