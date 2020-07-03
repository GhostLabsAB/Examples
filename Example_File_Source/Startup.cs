using GhostNodes.Std.BaseAdapter.Objects;
using GhostNodes.Std.Tracing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNodes.SourceAssembly
{
    public class Startup : SourceAdapter
    {
        private NodeParams m_NodeParams = null;

        public Startup()
        {
            Id = Guid.Parse("04496183-DFBD-4173-A8BE-1DBC9AA996B7");
            Description = "Source adapter for receiving messages from the local file system.";
            SupportsMultiThreading = true;
        }

        public override bool Initialize()
        {
            if (!IsInitialized)
            {
                m_NodeParams = NodeParams.Extract(NodeParamList);
            }

            return base.Initialize();
        }

        public override void ReceiveMessages()
        {
            m_NodeParams = NodeParams.Extract(NodeParamList);

            using (FileSystemEnumerator fse = new FileSystemEnumerator(m_NodeParams.Path, m_NodeParams.Filter, m_NodeParams.IncludeSubFolders))
            {
                fse.IncludeEmptyFiles = m_NodeParams.IncludeEmptyFiles;
                TimeSpan minfileAge = TimeSpan.FromSeconds(m_NodeParams.MinFileAge);

                var matches = fse.Matches(m_NodeParams.SortOptions, minfileAge, base.MaxReceiveCount);
                int matchesCount = matches.Count();
                if (matchesCount > 0)
                {
                    if (base.MultiThreadingEnabled && base.MaxThreads > 1)
                    {
                        var result = Parallel.ForEach(matches,
                            new ParallelOptions { MaxDegreeOfParallelism = base.MaxThreads, CancellationToken = CancellationToken },
                            (fi, state, index) =>
                            {
                                HandleMessage(fi);
                            });
                    }
                    else
                    {
                        foreach (FileInfo fi in matches)
                        {
                            // Break if we're stopping or suspending
                            if (!IsRunning || IsSuspended)
                                break;

                            HandleMessage(fi);
                        }
                    }
                }
            }
        }

        private void HandleMessage(FileInfo fi)
        {
            if (fi != null && fi.IsReadOnly == false)
            {
                AdapterMessage msg = new AdapterMessage(
                    File.ReadAllBytes(fi.FullName)
                );

                AddExpandableMessageParams(msg, fi);

                MessageReceived(msg, fi.FullName);
            }
        }

        public override void ConsumeReceivedMessage(AdapterMessage message, object state)
        {
            try
            {
                Tracer.WriteBegin();

                // Delete the file
                string filepath = state.ToString();
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

        public override void ReturnResponse(AdapterMessage message, object state)
        {
            try
            {
                Tracer.WriteBegin();
                if (m_NodeParams.IsTwoWay)
                {
                    m_NodeParams = NodeParams.Extract(NodeParamList);

                    string filepath = Path.Combine(m_NodeParams.ReplyPath, m_NodeParams.ReplyFilename);

                    Tracer.WriteLine($"Writing response-file: '{filepath}'");
                    System.IO.File.WriteAllBytes(filepath, message.GetDataAsArray());
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
