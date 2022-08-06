namespace Itask6.Models
{
    public class FakerOptions
    {
        public int length { get; set; }
        public string selectedCountry { get; set; }
        public double error { get; set; }
        public int seed {  get; set; }

        public FakerOptions(int length, string selectedCountry, double error, int seed)
        {
            this.length = length;
            this.selectedCountry = selectedCountry;
            this.error = error;
            this.seed = seed;
        }
    }
}
