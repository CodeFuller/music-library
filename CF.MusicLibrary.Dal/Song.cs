//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#pragma warning disable 1591 

namespace CF.MusicLibrary.Dal
{
    using System;
    using System.Collections.Generic;
    
    [System.CodeDom.Compiler.GeneratedCode("TextTemplatingFileGenerator", "")] 
    public partial class Song
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Song()
        {
            this.Playbacks = new HashSet<Playback>();
        }
    
        public int Id { get; set; }
        public int ArtistId { get; set; }
        public short OrderNumber { get; set; }
        public Nullable<short> Year { get; set; }
        public string Title { get; set; }
        public Nullable<int> GenreId { get; set; }
        public int Duration { get; set; }
        public Nullable<byte> Rating { get; set; }
        public int FileSize { get; set; }
        public Nullable<int> Bitrate { get; set; }
        public int DiscId { get; set; }
        public int PlaybacksCount { get; set; }
        public Nullable<System.DateTime> LastPlaybackTime { get; set; }
        public string Uri { get; set; }
    
        public virtual Artist Artist { get; set; }
        public virtual Genre Genre { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Playback> Playbacks { get; set; }
        public virtual Disc Disc { get; set; }
    }
}

#pragma warning restore 1591