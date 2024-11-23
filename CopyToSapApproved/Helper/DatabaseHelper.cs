using ClosedXML.Excel;
using CopyToSapApproved.Controllers;
using CopyToSapApproved.Models;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CopyToSapApproved.Helper;

public class DatabaseHelper
{
    private static string _connectionString;
    private static readonly string ResourcesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
    private static readonly string ResourcesPath1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
    private static readonly string dbFilePath = Path.Combine(ResourcesPath , "SAPFaster.db3");
    private static readonly string backupPath = Path.Combine(ResourcesPath1 , "SAPFasterBackup.db3");

    public DatabaseHelper()
    {
        _connectionString = $"Data Source={dbFilePath};Version=3;";
        EnsureDatabaseExists();
        CreateTables();
    }

    private void EnsureDatabaseExists()
    {
        if(!File.Exists(dbFilePath))
        {
            SQLiteConnection.CreateFile(dbFilePath);
        }
    }

    public static void ExecuteQuery(string query , object parameters = null)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.ExecuteNonQuery();
    }

    private void CreateTables()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        string createEmployeeTable = @"
                        CREATE TABLE IF NOT EXISTS Employee (
                        Code INTEGER PRIMARY KEY AUTOINCREMENT, 
                        Name TEXT NULL,
                        Job TEXT NULL, 
                        Branch TEXT NULL,
                        WorkLocation TEXT NULL,
                        Department TEXT NULL,
                        Vendor Text NULL
                    );";

        string createSparePartTable = @"
                    CREATE TABLE IF NOT EXISTS SparePart (
                        SapCode INTEGER PRIMARY KEY AUTOINCREMENT,
                        PartNo TEXT,
                        Group_ TEXT,
                        Model TEXT,
                        DescriptionAR TEXT,
                        DescriptionEN TEXT,
                        C1 TEXT,
                        C2 TEXT,
                        IsDamaged BOOLEAN
                    );";

        string createFinalNotesTable = @"
                    CREATE TABLE IF NOT EXISTS FinalNotes (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        SapNo INTEGER,
                        MiniNote TEXT,
                        Notes TEXT,
                        SAPStatus TEXT,                       
                        Explains TEXT,
                        IsFinished TEXT
                    );";

        string createCentersCycleTable = @"
                    CREATE TABLE IF NOT EXISTS CentersCycle (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,                       
                        ShortText TEXT
                    );";

        string createModelsModelsTable = @"
                    CREATE TABLE IF NOT EXISTS ModelsModels (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        MModels TEXT  ,
                        Part TEXT,                  
                        DescriptionAR TEXT            
                    );";

        string createMyNotesTable = @"
                    CREATE TABLE IF NOT EXISTS MyNotes (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT, 
                        MyNote TEXT,                        
                        CreatedAt DATETIME ,
                        AlertTime DATETIME,
                        Alerted Boolean
                    );";

        ExecuteNonQuery(connection , createEmployeeTable);
        ExecuteNonQuery(connection , createSparePartTable);
        ExecuteNonQuery(connection , createFinalNotesTable);
        ExecuteNonQuery(connection , createCentersCycleTable);
        ExecuteNonQuery(connection , createModelsModelsTable);
        ExecuteNonQuery(connection , createMyNotesTable);
    }

    private void ExecuteNonQuery(SQLiteConnection connection , string commandText)
    {
        using var command = new SQLiteCommand(commandText , connection);
        command.ExecuteNonQuery();
    }

    public static void ImportDatabase()
    {
        try
        {
            File.Copy(backupPath , dbFilePath , overwrite: true);
            MessageBox.Show("تم استيراد قاعدة البيانات بنجاح من " + backupPath);
        }
        catch(Exception ex)
        {
            MessageBox.Show("حدث خطأ أثناء استيراد قاعدة البيانات: " + ex.Message);
        }
    }

    public static void ExportDatabase()
    {
        try
        {
            File.Copy(dbFilePath , backupPath , overwrite: true);
            MessageBox.Show("تم تصدير قاعدة البيانات بنجاح إلى " + backupPath);
        }
        catch(Exception ex)
        {
            MessageBox.Show("حدث خطأ أثناء تصدير قاعدة البيانات: " + ex.Message);
        }
    }

    public bool CheckIfSapCodeExists(string tableName , int id)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        string query = $"SELECT COUNT(*) FROM {tableName} WHERE SapCode = @SapCode";

        using var command = new SQLiteCommand(query , connection);
        command.Parameters.AddWithValue("@SapCode" , id);
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public bool CheckIfModelsExists(string tableName , string mModels)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        string query = $"SELECT COUNT(*) FROM {tableName} WHERE MModels = @MModels";

        using var command = new SQLiteCommand(query , connection);
        command.Parameters.AddWithValue("@MModels" , mModels);
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public bool CheckIfIdExists(string tableName , int id)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        string query = $"SELECT COUNT(*) FROM {tableName} WHERE ID = @ID";

        using var command = new SQLiteCommand(query , connection);
        command.Parameters.AddWithValue("@ID" , id);
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public bool CheckIfCodeExists(string tableName , string code)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        string query = $"SELECT COUNT(*) FROM {tableName} WHERE Code = @Code";

        using var command = new SQLiteCommand(query , connection);
        command.Parameters.AddWithValue("@Code" , code);
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public bool CheckIfDamagedExists(string tableName , int SapCode)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        // استعلام للحصول على قيمة IsDamaged بناءً على الكود
        string query = $"SELECT IsDamaged FROM {tableName} WHERE SapCode = @SapCode LIMIT 1";

        using var command = new SQLiteCommand(query , connection);
        command.Parameters.AddWithValue("@SapCode" , SapCode);

        var result = command.ExecuteScalar();

        // إذا كانت النتيجة غير فارغة، قم بإرجاع القيمة كعدد صحيح (1 أو 0)، وإلا إرجاع 0
        if(result != DBNull.Value && result != null)
        {
            return Convert.ToBoolean(result);
        }

        return false; // إرجاع 0 إذا لم يتم العثور على الكود أو كانت القيمة فارغة
    }

    public void ClearTable(string tableName)
    {
        try
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string clearQuery = $"DELETE FROM {tableName};";

            using var command = new SQLiteCommand(clearQuery , connection);
            command.ExecuteNonQuery(); MessageBox.Show($"تم تفريغ جدول {tableName} بنجاح.");
        }
        catch(Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء تفريغ جدول {tableName}: " + ex.Message);
        }
    }

    public static void InsertIntoTable(string tableName , Dictionary<string , object> parameters)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        var columns = string.Join(", " , parameters.Keys);
        var values = string.Join(", " , parameters.Keys.Select(key => $"@{key}"));
        var insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

        using var command = new SQLiteCommand(insertQuery , connection);
        foreach(var param in parameters)
        {
            command.Parameters.AddWithValue($"@{param.Key}" , param.Value ?? DBNull.Value);
        }
        command.ExecuteNonQuery();
    }

    public static bool AddMyNote(string title , string myNote , DateTime alertTime)
    {
        try
        {
            var parameters = new Dictionary<string , object>
            {
                { "Title", title },
                { "MyNote", myNote },
                { "CreatedAt", DateTime.UtcNow },
                { "AlertTime ", alertTime}
            };

            InsertIntoTable("MyNotes" , parameters);

            MessageService.ShowMessage("تم اضافة الملحوظة بنجاح." , Brushes.LawnGreen);
            return true;
        }
        catch(Exception ex)
        {
            MessageBox.Show("حدث خطأ أثناء معالجة الملف: " + ex.Message);
            return false;
        }
    }

    //دالة لتعديل صف في أي جدول ***************
    public static void UpdateTableRow(string tableName , Dictionary<string , object> parameters , string conditionColumn , object conditionValue)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        var setClause = string.Join(", " , parameters.Keys.Select(key => $"{key} = @{key}"));
        var updateQuery = $"UPDATE {tableName} SET {setClause} WHERE {conditionColumn} = @{conditionColumn}";

        using var command = new SQLiteCommand(updateQuery , connection);
        foreach(var param in parameters)
        {
            command.Parameters.AddWithValue($"@{param.Key}" , param.Value ?? DBNull.Value);
        }
        command.Parameters.AddWithValue($"@{conditionColumn}" , conditionValue);

        command.ExecuteNonQuery();
    }

    // دالة لتعديل صف في جدول MyNotes بناءً على معرف ID
    public static bool UpdateMyNoteById(int id , string newTitle , string newNote , DateTime alertTime)
    {
        try
        {
            var parameters = new Dictionary<string , object>
        {
            { "Title", newTitle },
            { "MyNote", newNote },
            { "CreatedAt", DateTime.UtcNow }, // إضافة حقل لتاريخ التعديل إن كان مطلوبًا
                { "AlertTime ", alertTime}
        };

            UpdateTableRow("MyNotes" , parameters , "ID" , id);

            MessageService.ShowMessage($"تم تعديل الملحوظة برقم ID: {id} بنجاح." , Brushes.LawnGreen);
            return true;
        }
        catch(Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء تعديل الملحوظة: {ex.Message}");
            return false;
        }
    }

    public List<Dictionary<string , object>> GetAllData(string tableName)
    {
        var dataList = new List<Dictionary<string , object>>();

        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        string query = $"SELECT * FROM {tableName}";
        using var command = new SQLiteCommand(query , connection);
        using var reader = command.ExecuteReader();

        while(reader.Read())
        {
            var data = new Dictionary<string , object>();
            for(int i = 0; i < reader.FieldCount; i++)
            {
                data[reader.GetName(i)] = reader.GetValue(i);
            }
            dataList.Add(data);
        }
        return dataList;
    }

    public Dictionary<string , object> GetDataById(string tableName , int id , string idColumnName)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        string query = $"SELECT * FROM {tableName} WHERE {idColumnName} = @Id";
        using var command = new SQLiteCommand(query , connection);
        command.Parameters.AddWithValue("@Id" , id);

        using var reader = command.ExecuteReader();
        if(reader.Read())
        {
            var data = new Dictionary<string , object>();
            for(int i = 0; i < reader.FieldCount; i++)
            {
                data[reader.GetName(i)] = reader.GetValue(i);
            }
            return data;
        }

        return null;
    }

    public List<Dictionary<string , object>> GetDataIsFinished(string isFinishedValue)
    {
        var results = new List<Dictionary<string , object>>();

        // تحقق من أن جميع المدخلات غير فارغة
        if(string.IsNullOrWhiteSpace("FinalNotes") || string.IsNullOrWhiteSpace(isFinishedValue) || string.IsNullOrWhiteSpace("IsFinished"))
        {
            throw new ArgumentException("Table name, SapCode, and idColumnName must not be null or empty.");
        }

        try
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            // التأكد من أن أسماء الجداول والأعمدة لا تحتوي على أحرف خاصة لتجنب SQL Injection
            //string query = $"SELECT * FROM {tableName} WHERE {idColumnName} = @IsFinished";
            string query = $"SELECT * FROM FinalNotes WHERE IsFinished LIKE @IsFinished";

            using var command = new SQLiteCommand(query , connection);
            command.Parameters.AddWithValue("@IsFinished" , isFinishedValue);

            // تسجيل الاستعلام والقيم للتأكد
            Console.WriteLine($"Executing Query: {query} with IsFinished={isFinishedValue}");

            using var reader = command.ExecuteReader();
            while(reader.Read())  // استخدام while لجلب جميع الصفوف المطابقة
            {
                var data = new Dictionary<string , object>();
                for(int i = 0; i < reader.FieldCount; i++)
                {
                    data[reader.GetName(i)] = reader.GetValue(i);
                }
                results.Add(data);
            }
        }
        catch(Exception ex)
        {
            // التعامل مع الاستثناءات وفقاً للاحتياجات الخاصة بك (مثل التسجيل، إظهار رسالة للمستخدم، إلخ)
            Console.WriteLine($"Error: {ex.Message}");
        }

        if(results.Count == 0)
        {
            Console.WriteLine("No data found matching the criteria.");
        }

        return results;
    }

    // جلب العمود DescriptionAR فقط بدلاً من جميع الأعمدة
    public string GetDescriptionARFromSparePart(string SapCode)
    {
        string descriptionAR = null;

        // تحقق من أن المدخل غير فارغ
        if(string.IsNullOrWhiteSpace(SapCode))
        {
            throw new ArgumentException("SapCode must not be null or empty.");
        }

        try
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            // جلب العمود DescriptionAR فقط بدلاً من جميع الأعمدة
            string query = "SELECT DescriptionAR FROM SparePart WHERE SapCode = @SapCode";

            using var command = new SQLiteCommand(query , connection);
            command.Parameters.AddWithValue("@SapCode" , SapCode);

            MessageService.ShowMessage($"Executing Query: {query} with SapCode={SapCode}" , Brushes.LawnGreen);

            using var reader = command.ExecuteReader();
            if(reader.Read())  // قراءة الصف الأول المطابق فقط
            {
                descriptionAR = reader["DescriptionAR"]?.ToString();
            }
        }
        catch(Exception ex)
        {
            MessageService.ShowMessage($"Error: {ex.Message}" , Brushes.IndianRed);
        }

        if(descriptionAR == null)
        {
            MessageService.ShowMessage("لا يوجد بيانات تطابق الشرط." , Brushes.IndianRed);
        }

        return descriptionAR;
    }

    public static List<Dictionary<string , object>> SearchSpareParts(string sapCode , string group_ , string model , string descriptionAR)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        // تكوين جملة SQL الديناميكية
        var query = new StringBuilder("SELECT * FROM SparePart WHERE 1=1");

        // إضافة شروط البحث بناءً على الإدخالات المتاحة
        if(!string.IsNullOrEmpty(sapCode))
            query.Append(" AND SapCode LIKE @SapCode");

        if(!string.IsNullOrEmpty(group_))
            query.Append(" AND Group_ LIKE @Group_");

        if(!string.IsNullOrEmpty(model))
            query.Append(" AND Model LIKE @Model");

        if(!string.IsNullOrEmpty(descriptionAR))
            query.Append(" AND DescriptionAR LIKE @DescriptionAR");

        // تنفيذ الاستعلام
        using(var command = new SQLiteCommand(query.ToString() , connection))
        {
            // إضافة المعايير إلى الاستعلام
            if(!string.IsNullOrEmpty(sapCode))
                command.Parameters.AddWithValue("@SapCode" , "%" + sapCode + "%");

            if(!string.IsNullOrEmpty(group_))
                command.Parameters.AddWithValue("@Group_" , "%" + group_ + "%");

            if(!string.IsNullOrEmpty(model))
                command.Parameters.AddWithValue("@Model" , "%" + model + "%");

            if(!string.IsNullOrEmpty(descriptionAR))
                command.Parameters.AddWithValue("@DescriptionAR" , "%" + descriptionAR + "%");

            // قراءة النتائج من قاعدة البيانات
            var result = new List<Dictionary<string , object>>();
            using(var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    var sparePart = new Dictionary<string , object>
                {
                    { "SapCode", reader["SapCode"] is DBNull ? null : reader.GetInt32(0) },
                    { "PartNo", reader["PartNo"] is DBNull ? null : reader.GetString(1) },
                    { "Group_", reader["Group_"] is DBNull ? null : reader.GetString(2) },
                    { "Model", reader["Model"] is DBNull ? null : reader.GetString(3) },
                    { "DescriptionAR", reader["DescriptionAR"] is DBNull ? null : reader.GetString(4) },
                    { "DescriptionEN", reader["DescriptionEN"] is DBNull ? null : reader.GetString(5) },
                    { "C1", reader["C1"] is DBNull ? null : reader.GetString(6) },
                    { "C2", reader["C2"] is DBNull ? null : reader.GetString(7) },
                    { "IsDamaged", reader["IsDamaged"] is DBNull ? false : reader.GetBoolean(8) }
                };

                    result.Add(sparePart);
                }
            }

            return result;
        }
    }

    public static List<Dictionary<string , object>> SearchMyNotes(string title , string myNote)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        // تكوين جملة SQL الديناميكية
        var query = new StringBuilder("SELECT * FROM MyNotes WHERE 1=1");

        // إضافة شروط البحث بناءً على الإدخالات المتاحة         
        if(!string.IsNullOrEmpty(myNote))
            query.Append(" AND MyNote LIKE @MyNote");

        if(!string.IsNullOrEmpty(title))
            query.Append(" AND Title LIKE @Title");

        // تنفيذ الاستعلام
        using var command = new SQLiteCommand(query.ToString() , connection);

        // إضافة المعايير إلى الاستعلام
        if(!string.IsNullOrEmpty(myNote))
            command.Parameters.AddWithValue("@MyNote" , "%" + myNote + "%");

        if(!string.IsNullOrEmpty(title))
            command.Parameters.AddWithValue("@Title" , "%" + title + "%");

        // قراءة النتائج من قاعدة البيانات
        var result = new List<Dictionary<string , object>>();
        using var reader = command.ExecuteReader();
        while(reader.Read())
        {
            result.Add(new Dictionary<string , object>
            {
                {"ID", reader["ID"] is DBNull ? null : reader.GetInt32(0) },
                { "Title", reader["Title"] is DBNull ? null : reader.GetString(1) },
                { "MyNote", reader["MyNote"] is DBNull ? null : reader.GetString(2) },
                { "CreatedAt", reader["CreatedAt"] is DBNull ? null : reader.GetDateTime(3) }
            });
        }
        return result;
    }

    public static void DeleteRowFromMyNotes(int id)
    {
        try
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string deleteQuery = "DELETE FROM MyNotes WHERE ID = @ID;";

            using var command = new SQLiteCommand(deleteQuery , connection);
            command.Parameters.AddWithValue("@ID" , id);

            int rowsAffected = command.ExecuteNonQuery();

            if(rowsAffected > 0)
            {
                MessageBox.Show($"تم حذف الصف بنجاح برقم ID: {id}.");
            }
            else
            {
                MessageBox.Show("لم يتم العثور على صف بهذا الرقم.");
            }
        }
        catch(Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء حذف الصف: " + ex.Message);
        }
    }

    public static List<Dictionary<string , object>> SearchModelsModels(string mModels , string part)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        // تكوين جملة SQL الديناميكية
        var query = new StringBuilder("SELECT * FROM ModelsModels WHERE 1=1");

        // إضافة شروط البحث بناءً على الإدخالات المتاحة
        if(!string.IsNullOrEmpty(mModels))
            query.Append(" AND MModels LIKE @MModels");

        if(!string.IsNullOrEmpty(part))
            query.Append(" AND Part LIKE @Part");

        // تنفيذ الاستعلام
        using(var command = new SQLiteCommand(query.ToString() , connection))
        {
            // إضافة المعايير إلى الاستعلام
            if(!string.IsNullOrEmpty(mModels))
                command.Parameters.AddWithValue("@MModels" , "%" + mModels + "%");

            if(!string.IsNullOrEmpty(part))
                command.Parameters.AddWithValue("@Part" , "%" + part + "%");

            // قراءة النتائج من قاعدة البيانات
            var result = new List<Dictionary<string , object>>();
            using(var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    var modelsModels = new Dictionary<string , object>
                {
                    { "MModels", reader["MModels"] is DBNull ? null : reader.GetString(1) },
                    { "Part", reader["Part"] is DBNull ? null : reader.GetString(2) },
                    { "DescriptionAR", reader["DescriptionAR"] is DBNull ? null : reader.GetString(3) }
                };

                    result.Add(modelsModels);
                }
            }

            return result;
        }
    }

    public static List<Dictionary<string , object>> SearchEmployee(string code , string name , string branch)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        // تكوين جملة SQL الديناميكية
        var query = new StringBuilder("SELECT * FROM Employee WHERE 1=1");

        // إضافة شروط البحث بناءً على الإدخالات المتاحة
        if(!string.IsNullOrEmpty(code))
            query.Append(" AND Code LIKE @Code");

        if(!string.IsNullOrEmpty(name))
            query.Append(" AND Name LIKE @Name");

        if(!string.IsNullOrEmpty(branch))
            query.Append(" AND Branch LIKE @Branch");

        // تنفيذ الاستعلام
        using(var command = new SQLiteCommand(query.ToString() , connection))
        {
            // إضافة المعايير إلى الاستعلام
            if(!string.IsNullOrEmpty(code))
                command.Parameters.AddWithValue("@Code" , "%" + code + "%");

            if(!string.IsNullOrEmpty(name))
                command.Parameters.AddWithValue("@Name" , "%" + name + "%");

            if(!string.IsNullOrEmpty(branch))
                command.Parameters.AddWithValue("@Branch" , "%" + branch + "%");

            // قراءة النتائج من قاعدة البيانات
            var result = new List<Dictionary<string , object>>();
            using(var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    var employee = new Dictionary<string , object>
                {
                    { "Code", reader["Code"] is DBNull ? null : reader.GetInt32(0) },
                    { "Name", reader["Name"] is DBNull ? null : reader.GetString(1) },
                    { "Vendor", reader["Vendor"] is DBNull ? null : reader.GetString(6) }
                };

                    result.Add(employee);
                }
            }

            return result;
        }
    }

    public static List<Dictionary<string , object>> GetPartFromModelsModels()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        // تكوين جملة SQL الديناميكية مع استخدام DISTINCT للحصول على قيم فريدة من العمود Part
        var query = new StringBuilder("SELECT DISTINCT Part FROM ModelsModels WHERE 1=1");

        // تنفيذ الاستعلام
        using(var command = new SQLiteCommand(query.ToString() , connection))
        {
            // قراءة النتائج من قاعدة البيانات
            var result = new List<Dictionary<string , object>>();
            using(var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    var partData = new Dictionary<string , object>
                {
                    { "Part", reader["Part"] is DBNull ? null : reader.GetString(0) } // جلب البيانات الفريدة فقط من العمود Part
                };

                    result.Add(partData);
                }
            }

            return result;
        }
    }

    public static List<Dictionary<string , object>> GetBranchFromEmployee()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        // تكوين جملة SQL الديناميكية مع استخدام DISTINCT للحصول على قيم فريدة من العمود Part
        var query = new StringBuilder("SELECT DISTINCT Branch FROM Employee WHERE 1=1");

        // تنفيذ الاستعلام
        using(var command = new SQLiteCommand(query.ToString() , connection))
        {
            // قراءة النتائج من قاعدة البيانات
            var result = new List<Dictionary<string , object>>();
            using(var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    var branchData = new Dictionary<string , object>
                {
                    { "Branch", reader["Branch"] is DBNull ? null : reader.GetString(0) } // جلب البيانات الفريدة فقط من العمود Part
                };

                    result.Add(branchData);
                }
            }

            return result;
        }
    }

    public static List<Dictionary<string , object>> SearchCentersCycle(string shortText)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        // تكوين جملة SQL للبحث في حقل ShortText فقط
        var query = new StringBuilder("SELECT * FROM CentersCycle WHERE 1=1");

        // إضافة شرط البحث بناءً على shortText
        if(!string.IsNullOrEmpty(shortText))
            query.Append(" AND ShortText LIKE @ShortText");

        // تنفيذ الاستعلام
        using(var command = new SQLiteCommand(query.ToString() , connection))
        {
            // إضافة معايير البحث إلى الاستعلام
            if(!string.IsNullOrEmpty(shortText))
                command.Parameters.AddWithValue("@ShortText" , "%" + shortText + "%");

            // قراءة النتائج من قاعدة البيانات
            var result = new List<Dictionary<string , object>>();
            using(var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    // تعبئة النتائج في Dictionary
                    var centersCycle = new Dictionary<string , object>
                {
                    { "ID", reader["ID"] is DBNull ? null : reader.GetInt32(0) },
                    { "ShortText", reader["ShortText"] is DBNull ? null : reader.GetString(1) }
                };

                    result.Add(centersCycle);
                }
            }

            return result;
        }
    }

    public static List<Dictionary<string , object>> SearchFinalNotes(string notes)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        // تكوين جملة SQL للبحث في حقل ShortText فقط
        var query = new StringBuilder("SELECT * FROM FinalNotes WHERE 1=1");

        // إضافة شرط البحث بناءً على shortText
        if(!string.IsNullOrEmpty(notes))
            query.Append(" AND Notes LIKE @Notes");

        // تنفيذ الاستعلام
        using(var command = new SQLiteCommand(query.ToString() , connection))
        {
            // إضافة معايير البحث إلى الاستعلام
            if(!string.IsNullOrEmpty(notes))
                command.Parameters.AddWithValue("@Notes" , "%" + notes + "%");

            // قراءة النتائج من قاعدة البيانات
            var result = new List<Dictionary<string , object>>();
            using(var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    // تعبئة النتائج في Dictionary
                    var finalNotes = new Dictionary<string , object>
                    {
                        { "SAPStatus", reader["SAPStatus"] is DBNull ? null : reader.GetString(4) },
                        { "Notes", reader["Notes"] is DBNull ? null : reader.GetString(3) }
                    };

                    result.Add(finalNotes);
                }
            }

            return result;
        }
    }

    public List<Dictionary<string , object>> GetPendingNotifications()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        var query = "SELECT * FROM MyNotes WHERE AlertTime <= @Now AND Alerted = 0";
        using var command = new SQLiteCommand(query , connection);
        command.Parameters.AddWithValue("@Now" , DateTime.Now);

        using var reader = command.ExecuteReader();
        var result = new List<Dictionary<string , object>>();

        while(reader.Read())
        {
            var row = new Dictionary<string , object>();
            for(var i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i);
            }
            result.Add(row);
        }

        return result;
    }

    public void MarkNotificationAsShown(int id)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        var query = "UPDATE MyNotes SET Alerted = 1 WHERE ID = @Id";
        using var command = new SQLiteCommand(query , connection);
        command.Parameters.AddWithValue("@Id" , id);
        command.ExecuteNonQuery();
    }
}