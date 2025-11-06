using Microsoft.AspNetCore.Mvc;

namespace MovieApp.Areas.Public
{
    public class PublicAreaRegistration : AreaAttribute
    {
        public PublicAreaRegistration() : base("Public") { }
    }
}
