namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser {get;set;}
        public  int SourceUserId;

        public AppUser LikedUser {get;set;}

        public int LikedUserId   {get;set;}


    }
}