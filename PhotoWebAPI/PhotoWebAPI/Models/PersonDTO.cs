namespace PhotoWebAPI.Models
{
    public class PersonDTO
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }


        public PersonDTO(Person person)
        {
            Id = person.Id;
            Surname = person.Surname;
            Name = person.Name;
            Patronymic = person.Patronymic;
        }   
    }
}
