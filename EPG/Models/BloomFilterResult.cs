using System.ComponentModel.DataAnnotations;

namespace EPG.Models
{
    public enum BloomFilterResult
    {
        [Display(Name="Not Found")]
        NOTFOUND,
        [Display(Name = "Found")]
        FOUND,
        [Display(Name = "Not Safe")]
        NOTSAFE
    };
}
