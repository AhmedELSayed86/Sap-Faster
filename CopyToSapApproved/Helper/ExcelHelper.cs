using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace CopyToSapApproved.Helper;

public class ExcelHelper
{
    private static readonly string ResourcesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
    private static readonly string filePathSpareParts = Path.Combine(ResourcesPath , "SparePartsDetails.xlsx");
    private static readonly string filePathEmployee = Path.Combine(ResourcesPath , "EmployeeDetails.xlsx");
    private static readonly string filePathFinalNotes = Path.Combine(ResourcesPath , "FinalNotesDetails.xlsx");
    private static readonly string filePathCentersCycle = Path.Combine(ResourcesPath , "CentersCycleDetails.xlsx");
    private static readonly string filePathModelsModels = Path.Combine(ResourcesPath , "ModelsDetails.xlsx");

    private readonly DatabaseHelper _databaseHelper;

    public ExcelHelper(DatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;
    }

    public bool ImportSparePartsExcelFile()
    {
        return ImportSparePartsData(filePathSpareParts , "SpareParts");
    }

    public bool ImportEmployeeExcelFile()
    {
        return ImportEmployeeData(filePathEmployee , "Employee");
    }

    public bool ImportFinalNotesExcelFile()
    {
        return ImportFinalNotesData(filePathFinalNotes , "FinalNotes");
    }

    public bool ImportCentersCycleExcelFile()
    {
        return ImportCentersCycleData(filePathCentersCycle , "CentersCycle");
    }

    public bool ImportModelsModelsExcelFile()
    {
        return ImportModelsModelsData(filePathModelsModels , "ModelsModels");
    }

    private bool ImportSparePartsData(string excelFilePath , string sheetName)
    {
        if(!File.Exists(excelFilePath))
        {
            MessageBox.Show("الملف غير موجود: " + excelFilePath);
            return false;
        }

        try
        {
            using var workbook = new XLWorkbook(excelFilePath);
            var worksheet = workbook.Worksheet(sheetName);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // تخطي الصف الأول

            int skippedDueToExistence = 0;
            int skippedDueToInvalidData = 0;
            int AddedData = 0;

            foreach(var row in rows)
            {
                var SapCode = row.Cell(1).GetValue<string>();

                if(_databaseHelper.CheckIfSapCodeExists("SparePart" , Convert.ToInt32(SapCode)))
                {
                    skippedDueToExistence++;
                    continue;
                }

                var parameters = new Dictionary<string , object>
                    {
                        { "SapCode", SapCode },
                        { "PartNo", row.Cell(2).GetValue<string>() },
                        { "Group_", row.Cell(3).GetValue<string>() },
                        { "Model", row.Cell(4).GetValue<string>() },
                        { "DescriptionAR", row.Cell(5).GetValue<string>() },
                        { "DescriptionEN", row.Cell(6).GetValue<string>() },
                        { "C1", row.Cell(7).GetValue<string>() },
                        { "C2", row.Cell(8).GetValue<string>() },
                        { "IsDamaged", row.Cell(9).GetValue<bool>() }
                    };

                if(ValidateRow(parameters))
                {
                   DatabaseHelper.InsertIntoTable("SparePart" , parameters);
                    AddedData++;
                }
                else
                {
                    skippedDueToInvalidData++;
                }
            }

            MessageBox.Show($"تم أضافة ({AddedData}) صفوف بنجاح،\n" +
               $"تم تخطي عدد ({skippedDueToExistence}) صفوف بسبب ان الكود موجود بالفعل،\n" +
               $"وتم تخطي عدد ({skippedDueToInvalidData}) صفوف بسبب عدم توافق البيانات.");
            return true;
        }
        catch(Exception ex)
        {
            MessageBox.Show("حدث خطأ أثناء معالجة الملف: " + ex.Message);
            return false;
        }
    }

    private bool ImportEmployeeData(string excelFilePath , string sheetName)
    {
        if(!File.Exists(excelFilePath))
        {
            MessageBox.Show("الملف غير موجود: " + excelFilePath);
            return false;
        }

        try
        {
            using var workbook = new XLWorkbook(excelFilePath);
            var worksheet = workbook.Worksheet(sheetName);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // تخطي الصف الأول

            int skippedDueToExistence = 0;
            int skippedDueToInvalidData = 0;
            int AddedData = 0;

            foreach(var row in rows)
            {
                var code = row.Cell(1).GetValue<string>();

                if(_databaseHelper.CheckIfCodeExists("Employee" , code))
                {
                    skippedDueToExistence++;
                    continue;
                }

                var parameters = new Dictionary<string , object>
                {
                    { "Code", code },
                    { "Name", row.Cell(2).GetValue<string>() ?? string.Empty }, // وضع قيمة افتراضية إذا كانت الخلية فارغة              
                    { "Job", string.IsNullOrWhiteSpace(row.Cell(3).GetValue<string>()) ? null : row.Cell(3).GetValue<string>() },
                    { "Branch", string.IsNullOrWhiteSpace(row.Cell(4).GetValue<string>()) ? null : row.Cell(4).GetValue<string>() },
                    { "WorkLocation", string.IsNullOrWhiteSpace(row.Cell(5).GetValue<string>()) ? null : row.Cell(5).GetValue<string>() },
                    { "Department", string.IsNullOrWhiteSpace(row.Cell(6).GetValue<string>()) ? null : row.Cell(6).GetValue<string>() },
                    { "Vendor", string.IsNullOrWhiteSpace(row.Cell(7).GetValue<string>()) ? null : row.Cell(7).GetValue<string>() }
                };

                if(ValidateRow(parameters))
                {
                   DatabaseHelper.InsertIntoTable("Employee" , parameters);
                    AddedData++;
                }
                else
                {
                    skippedDueToInvalidData++;
                }
            }

            MessageBox.Show($"تم أضافة ({AddedData}) صفوف بنجاح،\n" +
                $"تم تخطي عدد ({skippedDueToExistence}) صفوف بسبب ان الكود موجود بالفعل،\n" +
                $"وتم تخطي عدد ({skippedDueToInvalidData}) صفوف بسبب عدم توافق البيانات.");
            return true;
        }
        catch(Exception ex)
        {
            MessageBox.Show("حدث خطأ أثناء معالجة الملف: " + ex.Message);
            return false;
        }
    }

    private bool ImportFinalNotesData(string excelFilePath , string sheetName)
    {
        if(!File.Exists(excelFilePath))
        {
            MessageBox.Show("الملف غير موجود: " + excelFilePath);
            return false;
        }

        try
        {
            using var workbook = new XLWorkbook(excelFilePath);
            var worksheet = workbook.Worksheet(sheetName);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // تخطي الصف الأول

            int skippedDueToExistence = 0;
            int skippedDueToInvalidData = 0;
            int AddedData = 0;

            foreach(var row in rows)
            {
                var sapNoStr = row.Cell(1).GetValue<string>();
                if(string.IsNullOrEmpty(sapNoStr) || !int.TryParse(sapNoStr , out int sapNo))
                {
                    skippedDueToInvalidData++;
                    continue;
                }

                //if(_databaseHelper.CheckIfIdExists("FinalNotes" , sapNo))
                //{
                //    skippedDueToExistence++;
                //    continue;
                //}

                var parameters = new Dictionary<string , object>
                    {
                        { "SapNo", sapNo },
                        { "MiniNote", row.Cell(2).GetValue<string>() },
                        { "Notes", row.Cell(3).GetValue<string>() },
                        { "SAPStatus", row.Cell(4).GetValue<string>() },
                        { "Explains", row.Cell(5).GetValue<string>() } ,
                        { "IsFinished", row.Cell(6).GetValue<string>() }
                    };

                if(ValidateRow(parameters))
                {
                   DatabaseHelper.InsertIntoTable("FinalNotes" , parameters);
                    AddedData++;
                }
                else
                {
                    skippedDueToInvalidData++;
                }
            }

            MessageBox.Show($"تم أضافة ({AddedData}) صفوف بنجاح،\n" +
                $"تم تخطي عدد ({skippedDueToExistence}) صفوف بسبب ان الكود موجود بالفعل،\n" +
                $"وتم تخطي عدد ({skippedDueToInvalidData}) صفوف بسبب عدم توافق البيانات.");
            return true;
        }
        catch(Exception ex)
        {
            MessageBox.Show("حدث خطأ أثناء معالجة الملف: " + ex.Message);
            return false;
        }
    }

    private bool ImportCentersCycleData(string excelFilePath , string sheetName)
    {
        if(!File.Exists(excelFilePath))
        {
            MessageBox.Show("الملف غير موجود: " + excelFilePath);
            return false;
        }

        try
        {
            using var workbook = new XLWorkbook(excelFilePath);
            var worksheet = workbook.Worksheet(sheetName);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // تخطي الصف الأول

            int skippedDueToExistence = 0;
            int skippedDueToInvalidData = 0;
            int AddedData = 0;

            foreach(var row in rows)
            {
                var iD = row.Cell(1).GetValue<string>();
                if(string.IsNullOrEmpty(iD) || !int.TryParse(iD , out int id))
                {
                    skippedDueToInvalidData++;
                    continue;
                }

                //if(_databaseHelper.CheckIfIdExists("FinalNotes" , sapNo))
                //{
                //    skippedDueToExistence++;
                //    continue;
                //}

                var parameters = new Dictionary<string , object>
                    {
                        { "ID", id },
                        { "ShortText", row.Cell(2).GetValue<string>() }
                    };

                if(ValidateRow(parameters))
                {
                    DatabaseHelper.InsertIntoTable("CentersCycle" , parameters);
                    AddedData++;
                }
                else
                {
                    skippedDueToInvalidData++;
                }
            }

            MessageBox.Show($"تم أضافة ({AddedData}) صفوف بنجاح،\n" +
                $"تم تخطي عدد ({skippedDueToExistence}) صفوف بسبب ان الكود موجود بالفعل،\n" +
                $"وتم تخطي عدد ({skippedDueToInvalidData}) صفوف بسبب عدم توافق البيانات.");
            return true;
        }
        catch(Exception ex)
        {
            MessageBox.Show("حدث خطأ أثناء معالجة الملف: " + ex.Message);
            return false;
        }
    }

    private bool ImportModelsModelsData(string excelFilePath , string sheetName)
    {
        if(!File.Exists(excelFilePath))
        {
            MessageBox.Show("الملف غير موجود: " + excelFilePath);
            return false;
        }

        try
        {
            using var workbook = new XLWorkbook(excelFilePath);
            var worksheet = workbook.Worksheet(sheetName);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // تخطي الصف الأول

            int skippedDueToExistence = 0;
            int skippedDueToInvalidData = 0;
            int AddedData = 0;

            foreach(var row in rows)
            {
                var mModels = row.Cell(1).GetValue<string>();

                if(_databaseHelper.CheckIfModelsExists("ModelsModels" , mModels))
                {
                    skippedDueToExistence++;
                    continue;
                }

                var parameters = new Dictionary<string , object>
                    { 
                        { "MModels", mModels },
                        { "Part", row.Cell(2).GetValue<string>() },
                        { "DescriptionAR", row.Cell(3).GetValue<string>() }
                    };

                if(ValidateRow(parameters))
                {
                   DatabaseHelper.InsertIntoTable("ModelsModels" , parameters);
                    AddedData++;
                }
                else
                {
                    skippedDueToInvalidData++;
                }
            }

            MessageBox.Show($"تم أضافة ({AddedData}) صفوف بنجاح،\n" +
               $"تم تخطي عدد ({skippedDueToExistence}) صفوف بسبب ان الكود موجود بالفعل،\n" +
               $"وتم تخطي عدد ({skippedDueToInvalidData}) صفوف بسبب عدم توافق البيانات.");
            return true;
        }
        catch(Exception ex)
        {
            MessageBox.Show("حدث خطأ أثناء معالجة الملف: " + ex.Message);
            return false;
        }
    }

    private bool ValidateRow(Dictionary<string , object> parameters)
    {
        foreach(var param in parameters)
        {
            if(param.Value == null || string.IsNullOrEmpty(param.Value.ToString()))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearTable(string tableName)
    {
        _databaseHelper.ClearTable(tableName);
    }
}