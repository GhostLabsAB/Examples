using GhostNodes.Std.BaseAdapter.Objects;
using System;
using System.ComponentModel;
using System.Composition;

namespace GhostNodes.DestinationAssembly
{
    [Export(typeof(INodeExpandableParametersBase))]
    public class ExpandableNodeParams : NodeExpandableParametersBase<ExpandableNodeParams>
    {
        [DisplayName("Name"), Description("Gets the name of the file."), Category("Expandable")]
        public string Name { get; set; }


        [DisplayName("FileName"), Description("Gets the name of the file."), Category("Expandable")]
        public string FileName { get; set; }


        [DisplayName("Extension"), Description("Gets the string representing the extension part of the file."), Category("Expandable")]
        public string Extension { get; set; }


        [DisplayName("FullName"), Description("Gets the full path of the directory or file."), Category("Expandable")]
        public string FullName { get; set; }


        [DisplayName("DirectoryName"), Description("Gets a string representing the directory's full path."), Category("Expandable")]
        public string DirectoryName { get; set; }


        [DisplayName("EncodingBodyName"), Description("Gets the current character encoding name."), Category("Expandable")]
        public string EncodingBodyName { get; set; }


        [DisplayName("CreationTime"), Description("Gets or sets the creation time of the current file or directory."), Category("Expandable")]
        public DateTime CreationTime { get; set; }


        [DisplayName("CreationTimeUtc"), Description("Gets or sets the creation time, in coordinated universal time (UTC), of the current file or directory."), Category("Expandable")]
        public DateTime CreationTimeUtc { get; set; }


        [DisplayName("LastWriteTime"), Description("Gets or sets the time when the current file or directory was last written to."), Category("Expandable")]
        public DateTime LastWriteTime { get; set; }


        [DisplayName("LastWriteTimeUtc"), Description("Gets or sets the time, in coordinated universal time (UTC), when the current file or directory was last written to."), Category("Expandable")]
        public DateTime LastWriteTimeUtc { get; set; }

    }

}
