# Npgsql Streaming Repro

You need a Postgres server available on localhost with credentials postgres/postgres 
and an empty database called test_db. Or modify the connection string in Program.cs
to point to a valid database.

Then run with `dotnet run` and observe the thrown exception: 
`An unhandled exception of type 'System.InvalidCastException' occurred in System.Private.CoreLib.dll: 'Can't write CLR type System.IO.MemoryStream with handler type ByteaHandler'`
