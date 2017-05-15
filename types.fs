namespace MetaTp

open System

type MetaParameter = {name: string; paratype: Type}
type MetaColumn = {name: string; coltype: Type}
type MetaTable = {name: string; columns: MetaColumn[]}
