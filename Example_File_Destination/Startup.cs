using GhostNodes.Std.BaseAdapter.Interfaces;
using GhostNodes.Std.BaseAdapter.Objects;
using GhostNodes.Std.BaseObjectModel.Objects;
using GhostNodes.Std.Tracing;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;

namespace GhostNodes.DestinationAssembly
{
    [Export(typeof(IDestinationAdapter))]
    public class Startup : DestinationAdapter
    {
        public Startup()
        {
            // This Guid need to be unique for every adapter
            Id = Guid.Parse("B003A596-C2D0-4AD9-BD9D-358DEA3C6498");
            Description = "Destination adapter for sending messages to the local file system.";
        }
        public override List<AdapterMessage> ProcessMessage(AdapterMessage message)
        {
            List<AdapterMessage> list = null;
            try
            {
                Tracer.WriteBegin();

                NodeParams p = NodeParams.Extract(message);
                
                if (p.Operation == FileOperationValues.Write)
                {
                    WriteFile(message, p);
                }
                else if (p.Operation == FileOperationValues.Read)
                {
                    list = ReadFiles(message, p, FileShare.Read);
                }
                else if (p.Operation == FileOperationValues.ReadAndDelete)
                {
                    list = ReadFiles(message, p, FileShare.None);
                }
                else  // FileOperationValues.None
                {
                    Tracer.WriteLine("File Operation set to 'None', so no action is done.");
                }
            }
            finally
            {
                Tracer.WriteEnd();
            }

            return list;
        }


        private void WriteFile(AdapterMessage message, NodeParams p)
        {
            lock (this)
            {
                string filepath = Path.Combine(p.Path, p.Filename);

                FileMode fileMode = FileMode.Create;
                if (p.AppendToExisting)
                    fileMode = FileMode.Append;

                // Create directory if it does not exist. This enables paths like 'c:\temp\%date%'
                if (Directory.Exists(p.Path) == false)
                    Directory.CreateDirectory(p.Path);

                using (FileStream fs = new FileStream(filepath, fileMode, FileAccess.Write, FileShare.None))
                {
                    // Seek to the end of the file in case we're appending to existing data
                    fs.Seek(0, SeekOrigin.End);
                    if (message.IsString)
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.Write(message.GetDataAsString());
                        }
                    }
                    else
                    {
                        byte[] buffer = message.GetDataAsArray();
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }


                // Preserve the creation time of source if it is configured to do so and we're not appending
                if (p.PreserveCreationTime && fileMode != FileMode.Append)
                {
                    string creationTime = message.Parameters.GetValueAsString("CreationTime");
                    if (creationTime != null)
                    {
                        try
                        {
                            DateTime time = DateTime.Parse(creationTime);
                            System.IO.File.SetCreationTime(filepath, time);
                        }
                        catch (Exception ex)
                        {
                            // Just skip if not possible
                            Tracer.WriteException(ex);
                        }
                    }
                }
            }
        }

        private List<AdapterMessage> ReadFiles(AdapterMessage original, NodeParams p, FileShare fileShare)
        {
            List<AdapterMessage> list = new List<AdapterMessage>();

            using (FileSystemEnumerator fse = new FileSystemEnumerator(p.Path, p.Filename, false))
            {
                foreach (FileInfo fi in fse.Matches())
                {
                    if (fi == null)
                        continue;

                    AdapterMessage msg = null;
                    if (!string.IsNullOrEmpty(p.FileMissingResponse) && !File.Exists(fi.FullName))
                    {
                        Tracer.WriteLine($"File '{fi.FullName}' is MISSING, \r\nsetting response content to: '{p.FileMissingResponse}'");
                        msg = AdapterMessageFactory.CreateResponseMessage(original, p.FileMissingResponse);
                    }
                    else if (!string.IsNullOrEmpty(p.EmptyFileResponse) && new FileInfo(fi.FullName).Length == 0)
                    {
                        Tracer.WriteLine($"File '{fi.FullName}' is EMPTY, \r\nsetting response content to: {p.EmptyFileResponse}");
                        msg = AdapterMessageFactory.CreateResponseMessage(original, p.EmptyFileResponse);
                    }
                    else
                    {
                        Tracer.WriteLine($"Reading file from: '{fi.FullName}'");

                        byte[] data = System.IO.File.ReadAllBytes(fi.FullName);

                        msg = AdapterMessageFactory.CreateResponseMessage(original, data);
                        AddExpandableMessageParams(msg, fi);
                    }

                    if (p.Operation == FileOperationValues.ReadAndDelete && !fi.IsReadOnly)
                    {
                        DeleteFile(fi.FullName);
                    }

                    list.Add(msg);
                }
            }


            return list;
        }

        public void DeleteFile(string filepath)
        {
            try
            {
                Tracer.WriteBegin();

                if (System.IO.File.Exists(filepath))
                {
                    Tracer.WriteLine($"Deleting file: '{filepath}'");
                    System.IO.File.Delete(filepath);
                }
            }
            finally
            {
                Tracer.WriteEnd();
            }
        }

        private void AddExpandableMessageParams(AdapterMessage msg, FileInfo fi)
        {
            SetMessageParameter(msg, "Name", fi.Name);
            SetMessageParameter(msg, "FileName", fi.Name);
            if (fi.Extension != null && fi.Extension.Length > 0)
            {
                SetMessageParameter(msg, "FileNameWithoutExtension", fi.Name.Replace(fi.Extension, ""));
                SetMessageParameter(msg, "Extension", fi.Extension);
            }
            else
            {
                SetMessageParameter(msg, "FileNameWithoutExtension", "");
                SetMessageParameter(msg, "Extension", "");
            }
            SetMessageParameter(msg, "FullName", fi.FullName);
            SetMessageParameter(msg, "DirectoryName", fi.DirectoryName);

            SetMessageParameter(msg, "CreationTime", fi.CreationTime.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
            SetMessageParameter(msg, "CreationTimeUtc", fi.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss.ffff"));

            SetMessageParameter(msg, "LastWriteTime", fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
            SetMessageParameter(msg, "LastWriteTimeUtc", fi.LastWriteTimeUtc.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
        }

    }
}
