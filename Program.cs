// See https://aka.ms/new-console-template for more information
using System.Data;
using System.Text;
using Npgsql;

const string connectionString = @"Host=localhost; Database=test_db; Username=postgres; Password=postgres";

using var connection = new NpgsqlConnection(connectionString);
connection.Open();
using var transaction = connection.BeginTransaction();

using (var tableCommand = new NpgsqlCommand(@"CREATE TABLE blob (id integer, bytes bytea)", connection))
{
    tableCommand.ExecuteNonQuery();
}

using (var directWriteCommand = new NpgsqlCommand(@"INSERT INTO blob VALUES (1, convert_to('a', 'LATIN1'))", connection))
{
    directWriteCommand.ExecuteNonQuery();
}

using (var directReadCommand = new NpgsqlCommand(@"SELECT bytes FROM blob WHERE id = 1", connection))
{
    using var directReadReader = directReadCommand.ExecuteReader();
    directReadReader.Read();
    using var directReadStream = directReadReader.GetStream(0);
    using var directReadStreamReader = new StreamReader(directReadStream);
    var s = directReadStreamReader.ReadToEnd();
    if (s == "a")
    {
        Console.WriteLine($"✔ Got correct data: '{s}'");
    }
    else 
    {
        Console.WriteLine($"❌ Got invalid data: '{s}'");
    }
}

using (var writeCommand = new NpgsqlCommand(@"INSERT INTO blob VALUES (2, @bytes)", connection))
{
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes("b"));
    writeCommand.Parameters.Add(new NpgsqlParameter
    {
        DbType = DbType.Binary,
        ParameterName = "@bytes",
        Value = stream,
        Size = -1,
    });
    writeCommand.ExecuteNonQuery();
}

using (var streamReadCommand = new NpgsqlCommand(@"SELECT bytes FROM blob WHERE id = 2", connection))
{
    using var streamReadReader = streamReadCommand.ExecuteReader();
    streamReadReader.Read();
    using var streamReadStream = streamReadReader.GetStream(0);
    using var streamReadStreamReader = new StreamReader(streamReadStream);
    var s = streamReadStreamReader.ReadToEnd();
    if (s == "b")
    {
        Console.WriteLine($"✔ Got correct data: '{s}'");
    }
    else 
    {
        Console.WriteLine($"❌ Got invalid data: '{s}'");
    }
}
