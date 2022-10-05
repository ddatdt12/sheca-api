﻿using System.ComponentModel.DataAnnotations;

namespace Sheca.Dtos
{
    public class UpdateCourseDto
    {
        public string? Title { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        //seconds
        [Range(0, 86400, ErrorMessage = "StartTime is between 0 - 86400")]
        public int? StartTime { get; set; }
        [Range(0, 86400, ErrorMessage = "EndTime is between 0 - 86400")]
        public int? EndTime { get; set; }
        public int? NumOfLessonsPerDay { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NumOfLessons { get; set; }
        public int? NotiBeforeTime { get; set; }
        public string? ColorCode { get; set; }
    }
}