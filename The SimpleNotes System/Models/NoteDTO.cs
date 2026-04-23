using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace The_SimpleNotes_System.Models
{
    public class NoteDto
    { 
        [JsonPropertyName("id")] 
        public string? Id { get; set; }
        [JsonPropertyName("title")] 
        public string? Title { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; } 
        [JsonPropertyName("status")] 
        public string? Status { get; set; } }
}
