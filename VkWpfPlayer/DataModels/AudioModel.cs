namespace VkWpfPlayer.DataModels
{
    public class AudioModel
    {

        public long Owner_ID { get; set; }
        public int DurationSeconds { get; set; }
        public long ID { get; set; }
        public string Title { get; set; }
        public string AudioUrl { get; set; }
        public string AccessKey { get; set; }

        public string Artist { get; set; }
        public string ImageUrl { get; set; }



    }
}
