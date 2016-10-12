using UnityEngine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using MySql;
using MySql.Data.MySqlClient;

public class DatabaseHandlerMysql : MonoBehaviour {

    public string host, database, user, password;
    public bool pooling = true;

    private string _connectionString;
    private MySqlConnection _msCon = null;
    private MySqlCommand _msCmd = null;
    private MySqlDataReader _msDataReader = null;

    private MD5 _md5Hash;

    void Awake() {
        //Don't destroy this important gameobject
        DontDestroyOnLoad(this.gameObject);
        _connectionString = "Server=" + host + ";Database=" + database + ";User=" + user + ";Password=" + password + ";Pooling=";

        _connectionString += pooling ? "true;" : "false;";

        try {
            _msCon = new MySqlConnection(_connectionString);
            _msCon.Open();
            Debug.Log("MySql State: "+_msCon.State);
        }
        catch (Exception e) {
            Debug.Log(e);
        }
    }

    public string RunQuery(string query) {
        _msCmd = _msCon.CreateCommand();
        _msCmd.CommandText = query;
        _msDataReader = _msCmd.ExecuteReader();

        string runquery = "";

        List<string> columns = new List<string>();

        for (int i = 0; i < _msDataReader.FieldCount; i++)
        {
            columns.Add(_msDataReader.GetName(i));
        }

        while (_msDataReader.Read())
        {
            for (int i = 0; i < _msDataReader.FieldCount; i++)
            {
                runquery += _msDataReader[columns[i]] + " , ";
            }
        }
        return runquery;
    }

    void OnApplicationQuit() {
        if (_msCon != null) {
            if (_msCon.State.ToString() != "closed") {
                _msCon.Close();
                Debug.Log("MySQL Connection Closed");
            }

            _msCon.Dispose();
        }
    }

    public string GetConnectionState() {
        return _msCon.State.ToString();
    }
}
