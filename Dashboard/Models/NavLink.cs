namespace Dashboard.Models;

public class NavLink
{
    public String Name { get; set; }
    public String Url { get; set; }
    
    public NavLink(String name, String url)
    {
        this.Name = name;
        this.Url = url;
    }
}