using System.Data;
using System.Data.Odbc;

namespace StreamProcessing.SqlExecutor.Interfaces;

internal interface IParameterCommandCreator
{
    IDbCommand Create(OdbcConnection connection, string commandText, 
        IReadOnlyCollection<string>? ParameterFileds, 
        IReadOnlyDictionary<string, object>? record);
}