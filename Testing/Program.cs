using System;
using System.Data.SqlClient;
using System.Web;

namespace Testing
{
    public static class Program
    {
        private static string _cnnString = "Provider=SQLOLEDB;Data Source=delico;uid=SQL_Aliado;Password=SQL_Aliado;Initial Catalog=AmbitoV2; Application Name=AmbitoJuridico;".AdoCnnToNetCnn();

        public static void Main(string[] args)
        {
            var a = new Uri("http://facebook.com?name=Paola&age=22").AddQuery("age", "19");

            var b = EWeb.AddQuery("http://facebook.com?name=Paola", "age", "20");
            ;
        }
    }
}
