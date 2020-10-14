using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace LogCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("InfinityLogCollector v.1.0");
            string logs = "C:\\Program Files (x86)\\IntelTelecom\\Infinity Call-center X";
            string telephonypath;
            string infinityxpath;
            string savelogpath;
            string archive;

            try
            {
                string[] Mass = File.ReadAllLines(@"C:\\Program Files (x86)\\IntelTelecom\\Infinity Call-center X\\CxTelephony\\conf\\log.ini", System.Text.Encoding.Default);
                telephonypath = Mass[1].Remove(0, 5);
                if (telephonypath == "logs/")
                {
                    telephonypath = logs + "\\CxTelephony\\logs";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error.Не найден файл log.ini.Установлен путь по умолчанию" + ex);
                telephonypath = logs + "\\CxTelephony\\logs";
            }


            try
            {

                // Connect to a PostgreSQL database
                NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=10000;User Id=cxdbuser; " +
                   "Password=cxdbwizard;Database=Cx_Configuration;");
                conn.Open();

                // Define a query returning a single row result set
                NpgsqlCommand command = new NpgsqlCommand("SELECT \"ValueString\" FROM \"C_ParamsValues\" WHERE \"IDParam\" = 10000071", conn);

                // Execute the query and obtain the value of the first column of the first row
                string paramvalue = (String)command.ExecuteScalar();

                if (String.IsNullOrEmpty(paramvalue))
                {
                    infinityxpath = "C:\\Program Files (x86)\\IntelTelecom\\Infinity Call-center X\\Server\\Data";

                }
                else
                {
                    infinityxpath = paramvalue;
                }



                conn.Close();

            }
            catch (Exception ex)
            {

                Console.WriteLine("Ошибка получения пути логов Infinity X. Указан путь по умолчанию" + ex);
                infinityxpath = "C:\\Program Files (x86)\\IntelTelecom\\Infinity Call-center X\\Server\\Data";
            }

            Console.WriteLine("Определен путь к логам telephony: " + telephonypath);
            Console.WriteLine("Определен путь логов Infinity X: " + infinityxpath);
            Console.WriteLine("Сохранить в: ");
            
            savelogpath =Console.ReadLine();


            if (savelogpath is null)
            {
                Console.WriteLine("Не выбран путь сохранения логов в поле Куда");

            }

            if (savelogpath == "")
            {
                Console.WriteLine("Не выбран путь сохранения логов в поле Куда");

            }
            else
            {
                
                // Create temp folder
                try
                {
                    Directory.CreateDirectory(savelogpath + "\\" + "temp");
                    
                }

                catch (IOException createError)
                {
                    Console.WriteLine(createError.Message);
                }


                string sourceDir = infinityxpath;
                string backupDir = savelogpath + "\\" + "temp";

                try
                {

                    string[] txtList = Directory.GetFiles(sourceDir, "*.log");


                    // Copy text files.
                    foreach (string f in txtList)
                    {

                        // Remove path from the file name.
                        string fName = f.Substring(sourceDir.Length + 1);

                        try
                        {
                            // Will not overwrite if the destination file already exists.
                            File.Copy(Path.Combine(sourceDir, fName), Path.Combine(backupDir, fName));
                        }

                        // Catch exception if the file was already copied.
                        catch (IOException copyError)
                        {
                            Console.WriteLine(copyError.Message);
                        }
                    }


                    archive = savelogpath + "\\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".zip";

                    ZipFile.CreateFromDirectory(backupDir, archive);
                    
                    Directory.Delete(backupDir, true);


                    /*
                    // Delete source files that were copied.
                    foreach (string f in txtList)
                    {
                        File.Delete(f);
                    }
                    */

                }

                catch (DirectoryNotFoundException dirNotFound)
                {
                    Console.WriteLine("Ошибка при сохранении лог файлов" + dirNotFound);
                }



            }



        }
    }
}
