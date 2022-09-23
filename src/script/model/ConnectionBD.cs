using Godot;
using System;
using MySql.Data;

public class ConnectionBD
{
    public static SqlConnection openDatabaseConnection()
    {
        SqlConnection connection = new SqlConnection(CONNECTION_STRING);
        return connection;
    }
}
