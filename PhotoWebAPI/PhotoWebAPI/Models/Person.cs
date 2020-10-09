namespace PhotoWebAPI
{
    public class Person
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }

        public Person()
        { }

        //public List<Photo> Photos { get; set; } 
        // Потиворечит ТЗ, по ТЗ в структуре данных Person нет информации про аватары (фотографии)
    }
}
