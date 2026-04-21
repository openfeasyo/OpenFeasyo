using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace GhostlyGame.Models
{
    /// <summary>
    /// Represents a patient in the system.
    /// Note: therapist_id now references user_profiles.id (not the old therapists table)
    /// </summary>
    [Table("patients")]
    public class Patient : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("therapist_id")]
        public Guid TherapistId { get; set; }

        [Column("patient_code")]
        public string PatientCode { get; set; }

        [Column("age_group")]
        public string AgeGroup { get; set; }

        [Column("gender")]
        public string Gender { get; set; }

        [Column("pathology_category")]
        public string PathologyCategory { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("current_mvc75_ch1")]
        public float? CurrentMvc75Ch1 { get; set; }

        [Column("current_mvc75_ch2")]
        public float? CurrentMvc75Ch2 { get; set; }

        [Column("current_target_ch1_ms")]
        public float? CurrentTargetCh1Ms { get; set; }

        [Column("current_target_ch2_ms")]
        public float? CurrentTargetCh2Ms { get; set; }

        [Column("current_difficulty_level")]
        public int? CurrentDifficultyLevel { get; set; }

        [Column("level_to_play")]
        public int? LevelToPlay { get; set; }

    }
}
