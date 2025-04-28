using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RubricaContatti
{
    class Program
    {
        static List<Contact> contacts = new List<Contact>();
        static string filePath = "rubrica.json";

        static void Main(string[] args)
        {
            contacts = LoadContacts();
            SortContacts();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Rubrica Contatti:");
                Console.WriteLine("1. Aggiungi contatto");
                Console.WriteLine("2. Visualizza contatti");
                Console.WriteLine("3. Cerca contatto");
                Console.WriteLine("4. Modifica contatto");
                Console.WriteLine("5. Elimina contatto");
                Console.WriteLine("6. Esci");
                Console.Write("Scegli un'opzione: ");

                string choice = Console.ReadLine()?.Trim();
                if (choice == null)
                {
                    Console.WriteLine("Input non valido. Riprova.");
                    continue;
                }

                switch (choice)
                {
                    case "1":
                        AddContact();
                        break;
                    case "2":
                        ShowContacts();
                        break;
                    case "3":
                        SearchContact();
                        break;
                    case "4":
                        EditContact();
                        break;
                    case "5":
                        DeleteContact();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Scelta non valida.");
                        Pause();
                        break;
                }
            }
        }

        static void AddContact()
        {
            Console.Write("Inserisci il nome: ");
            string name = Console.ReadLine();
            Console.Write("Inserisci il telefono: ");
            string phone = Console.ReadLine();
            Console.Write("Inserisci l'email: ");
            string email = Console.ReadLine();

            contacts.Add(new Contact { Name = name, Phone = phone, Email = email });
            SortContacts();
            SaveContacts();
            Console.WriteLine("Contatto aggiunto con successo!");
            Pause();
        }

        static void ShowContacts()
        {
            Console.WriteLine("\nLista contatti:");
            if (contacts.Count == 0)
            {
                Console.WriteLine("Nessun contatto presente.");
            }
            else
            {
                foreach (var contact in contacts)
                {
                    Console.WriteLine($"Nome: {contact.Name}, Telefono: {contact.Phone}, Email: {contact.Email}");
                }
            }
            Pause();
        }

        static void SearchContact()
        {
            Console.Write("Inserisci il nome da cercare: ");
            string name = Console.ReadLine();

            var found = contacts.FindAll(c => c.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
            if (found.Count > 0)
            {
                Console.WriteLine("\nContatti trovati:");
                foreach (var contact in found)
                {
                    Console.WriteLine($"Nome: {contact.Name}, Telefono: {contact.Phone}, Email: {contact.Email}");
                }
            }
            else
            {
                Console.WriteLine("Contatto non trovato.");
            }
            Pause();
        }

        static void EditContact()
        {
            Console.Write("Inserisci il nome del contatto da modificare: ");
            string name = Console.ReadLine();

            var matches = contacts.FindAll(c => c.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);

            if (matches.Count == 0)
            {
                Console.WriteLine("Nessun contatto trovato.");
            }
            else if (matches.Count == 1)
            {
                ModifyContact(matches[0]);
            }
            else
            {
                Console.WriteLine("\nSono stati trovati più contatti:");
                for (int i = 0; i < matches.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. Nome: {matches[i].Name}, Telefono: {matches[i].Phone}, Email: {matches[i].Email}");
                }

                Console.Write("Inserisci il numero del contatto da modificare: ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= matches.Count)
                {
                    ModifyContact(matches[choice - 1]);
                }
                else
                {
                    Console.WriteLine("Scelta non valida.");
                }
            }
            Pause();
        }

        static void ModifyContact(Contact contact)
        {
            Console.WriteLine($"Contatto selezionato: {contact.Name}, {contact.Phone}, {contact.Email}");
            Console.Write("Inserisci il nuovo nome (lascia vuoto per mantenere): ");
            string newName = Console.ReadLine();
            Console.Write("Inserisci il nuovo telefono (lascia vuoto per mantenere): ");
            string newPhone = Console.ReadLine();
            Console.Write("Inserisci il nuovo email (lascia vuoto per mantenere): ");
            string newEmail = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(newName))
                contact.Name = newName;
            if (!string.IsNullOrWhiteSpace(newPhone))
                contact.Phone = newPhone;
            if (!string.IsNullOrWhiteSpace(newEmail))
                contact.Email = newEmail;

            SortContacts();
            SaveContacts();
            Console.WriteLine("Contatto aggiornato con successo!");
        }

        static void DeleteContact()
        {
            Console.Write("Inserisci il nome del contatto da eliminare: ");
            string name = Console.ReadLine();

            var matches = contacts.FindAll(c => c.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);

            if (matches.Count == 0)
            {
                Console.WriteLine("Nessun contatto trovato.");
            }
            else if (matches.Count == 1)
            {
                contacts.Remove(matches[0]);
                SortContacts();
                SaveContacts();
                Console.WriteLine("Contatto eliminato con successo!");
            }
            else
            {
                Console.WriteLine("\nSono stati trovati più contatti:");
                for (int i = 0; i < matches.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. Nome: {matches[i].Name}, Telefono: {matches[i].Phone}, Email: {matches[i].Email}");
                }

                Console.Write("Inserisci il numero del contatto da eliminare: ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= matches.Count)
                {
                    contacts.Remove(matches[choice - 1]);
                    SortContacts();
                    SaveContacts();
                    Console.WriteLine("Contatto eliminato con successo!");
                }
                else
                {
                    Console.WriteLine("Scelta non valida.");
                }
            }
            Pause();
        }

        static void Pause()
        {
            Console.WriteLine("\nPremi invio per continuare...");
            Console.ReadLine();
        }

        static void SaveContacts()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(contacts, options);
            File.WriteAllText(filePath, json);
        }

        static List<Contact> LoadContacts()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<Contact>>(json);
            }
            else
            {
                return new List<Contact>();
            }
        }

        static void SortContacts()
        {
            contacts.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
        }
    }

    class Contact
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
