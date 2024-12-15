using System.Diagnostics.Tracing;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Transactions;

namespace LibarayConsoleApplication
{
    class Book
    {
        public int BookId { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public bool isAvailable { get; set; }

        public void displayBookDetails()
        {
            Console.Write($"\n Book Id = {BookId} \n Title = {Title} \n Author = {Author} \n Genre = {Genre} \n isAvailable = {isAvailable}");
        }
    }

    class Borrower
    {
        public int BorrowerId { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; }
    }

    class Transaction
    {
        public int TransactionId { get; set; }
        public int BookId { get; set; }
        public int BorrowerId { get; set; }
        public DateTime Date { get; set; }
        public bool isBorrowed { get; set; }
    }

    class Library
    {

        public void displayMenu()
        {
            Console.Write("\n Select an option : ");
            Console.Write("\n 1.Add a new book\r\n 2.Remove a book\r\n 3.Update a book\r\n 4.Register a new borrower\r\n 5.Update a borrower\r\n 6.Delete a borrower\r\n 7.Borrow a book\r\n 8.Return a book\r\n 9.Search for books by title, author, or genre\r\n 10.View all books\r\n 11.View borrowed books by a specific borrower\r\n 12.Exit the application \n : ");
            return;
        }


        // Works fine
        public void AddBook(Book book)
        {
            // First cheking whether book with same id already exists in file or not
            FileStream fin = new("Books.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new(fin);
            string data;
            while((data= sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');
                if (int.Parse(tokens[0]) == book.BookId)
                {
                    Console.WriteLine($"\nBook having id {book.BookId} already exists!");
                    return;
                }
            }
            sr.Close();
            fin.Close();

            fin = new FileStream("Books.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fin);

            data = $"{book.BookId},{book.Title},{book.Author},{book.Genre},{book.isAvailable}";
            sw.WriteLine(data);
            Console.WriteLine("Book added successfully!");
            sw.Close();
            fin.Close();
        }

        // Works fine
        public void RemoveBook(int bookid)
        {
            FileStream fin = new FileStream("Books.txt", FileMode.Open, FileAccess.Read);
            FileStream fin2 = new FileStream("TempBooks.txt", FileMode.Create, FileAccess.Write);

            StreamReader sr = new StreamReader(fin);
            StreamWriter sw = new StreamWriter(fin2);

            string data;
            bool flag = false;

            while ((data = sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');
                if (int.Parse(tokens[0]) == bookid && bool.Parse(tokens[4]) == true)
                {
                    //Console.WriteLine($"\n Book having id {bookid} found successfully!");
                    flag = true;
                    continue; // skipping this iteration
                }
                else
                {
                    sw.WriteLine(data); // writing in TempBooks.txt
                }
            }

            sr.Close();
            sw.Close();
            fin.Close();
            fin2.Close();

            if (!flag)
            {
                Console.WriteLine($"\n Book having id {bookid} not found!");
            }
            else
            {
                // now need to copy data from TempBooks.txt back to Books.txt
                fin = new FileStream("Books.txt", FileMode.Create, FileAccess.Write);
                fin2 = new FileStream("TempBooks.txt", FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fin2);
                sw = new StreamWriter(fin);

                while ((data = sr.ReadLine()) != null)
                {
                    sw.WriteLine(data); // writing data to Books.txt
                }

                sr.Close();
                sw.Close();
                fin.Close();
                fin2.Close();
            }

            Console.WriteLine($"\n Book having id {bookid} deleted successfully!");
            File.Delete("TempBooks.txt"); // deleting TempData.txt file after use
        }

        // Works fine
        public void UpdateBook(int id)
        {
            FileStream fin = new FileStream("Books.txt", FileMode.Open, FileAccess.Read);
            FileStream fin2 = new FileStream("TempBooks.txt", FileMode.Create, FileAccess.Write); 
            StreamWriter sw = new StreamWriter(fin2);
            StreamReader sr = new StreamReader(fin);

            string data;
            bool flag = false;

            while ((data = sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');
                if (int.Parse(tokens[0]) == id) // means book has been found in library
                {
                    flag = true;
                    Console.WriteLine($"\n Book having id {id} found in library!");
                    Console.WriteLine($"\n Enter new information regarding book id {id} in the format(title, author, genre): ");
               
                    string _title, _author, _Genre;
                    bool _isAvaialable;
                    Console.Write("\n Enter new info to update : ");
                    _title = Console.ReadLine();
                    _author = Console.ReadLine();
                    _Genre = Console.ReadLine();
                    _isAvaialable = bool.Parse(Console.ReadLine());
                    string _data = $"{id},{_title},{_author},{_Genre},{_isAvaialable}";
                    sw.WriteLine(_data);
                }
                else
                {
                    sw.WriteLine(data); // writing same data as it is in TempBooks.txt
                }
            }
            sr.Close();
            sw.Close();
            fin.Close();
            fin2.Close();

            if(!flag)
            {
                Console.WriteLine($"\n Book having id {id} not found in library!");
            }
            else
            {
                // means book was found in library and new info needs to be stored from TempBooks.txt back to Books.txt
                fin = new FileStream("Books.txt", FileMode.Create, FileAccess.Write);
                fin2 = new FileStream("TempBooks.txt", FileMode.Open, FileAccess.Read);

                sw = new StreamWriter(fin);
                sr = new StreamReader(fin2);

                while((data = sr.ReadLine()) != null)
                {
                    sw.WriteLine(data);
                }

                sr.Close();
                sw.Close();
                fin.Close();
                fin2.Close();
            }

            Console.WriteLine("\n Updation successfull!");
            File.Delete("TempBooks.txt");
        }

        // Works fine
        public List<Borrower> GetAllBorrowers()
        {

            FileStream fin = new FileStream("Borrowers.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fin);

            List<Borrower> borrowers = new List<Borrower>();

            string data;
            while ((data = sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');
                Borrower b = new Borrower();
                b.BorrowerId = int.Parse(tokens[0]);
                b.Name = tokens[1];
                b.Email = tokens[2];
                borrowers.Add(b);
                //books.Add(book);
            }

            sr.Close();
            fin.Close();
            return borrowers;
        }

        // Works fine
        public List<Book> GetAllBooks()
        {

            FileStream fin = new FileStream("Books.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fin);

            List<Book> books = new List<Book>();

            string data;
            while ((data = sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');
                Book book = new Book
                {
                    BookId = int.Parse(tokens[0]),
                    Title = tokens[1],
                    Author = tokens[2],
                    Genre = tokens[3],
                    isAvailable = bool.Parse(tokens[4])
                };
                
                Console.WriteLine(book.Author);
                books.Add(book);
            }

            sr.Close();
            fin.Close();
            return books;
        }

        // Works fine
        public Book GetBookById(int bookid)
        {
            FileStream fin = new FileStream("Books.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fin);

            string data;
            while ((data = sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');
                if (int.Parse(tokens[0]) == bookid)
                {
                    Console.WriteLine($"\n Book having id {bookid} found!");
                    Book book = new Book
                    {
                        BookId = int.Parse(tokens[0]),
                        Title = tokens[1],
                        Author = tokens[2],
                        Genre = tokens[3],
                        isAvailable = bool.Parse(tokens[4])
                    };

                    return book;
                }
            }

            Console.WriteLine($"\n Book having id {bookid} not found!");
            return null;
        }

        // WOrks fine
        public List<Book> SearchBook(string query)
        {
            FileStream fin = new FileStream("Books.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fin);

            List<Book> books = new List<Book>();

            string data;
            while ((data = sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');
                if (tokens[1] == query || tokens[2] == query || tokens[3] == query)
                {
                    Book book = new Book
                    {
                        BookId = int.Parse(tokens[0]),
                        Title = tokens[1],
                        Author = tokens[2],
                        Genre = tokens[3],
                        isAvailable = bool.Parse(tokens[4])
                    };

                    books.Add(book);
                }
            }
            sr.Close();
            fin.Close();

            return books;
        }

        // Works fine
        public void RegisterBorrower(Borrower borrower)
        {

            FileStream fin = new FileStream("Borrowers.txt", FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(fin);

            string data;
            while ((data= sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');

                if (int.Parse(tokens[0]) == borrower.BorrowerId)
                {
                    Console.WriteLine($"\n Cannot register a borrower because borrwer with id {borrower.BorrowerId} already exits!");
                    sr.Close();
                    fin.Close();
                    return;
                }
            }

            sr.Close();
            fin.Close();
            fin = new FileStream("Borrowers.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fin);
            data = $"{borrower.BorrowerId},{borrower.Name},{borrower.Email}";
            sw.WriteLine(data);
            Console.WriteLine("\n Borrower registered successfully!");
            sw.Close();
            fin.Close();
        }

        // WOrks fine
        public void UpdateBorrower(int id) 
        {
            FileStream fin = new FileStream("Borrowers.txt", FileMode.OpenOrCreate, FileAccess.Read);
            FileStream fin2 = new FileStream("Temp.txt", FileMode.Create, FileAccess.Write);
            StreamReader sr = new StreamReader(fin);
            StreamWriter sw = new StreamWriter(fin2);

            string data;
            bool flag = false;

            while((data = sr.ReadLine() ) != null)
            {
                var tokens = data.Split(',');
                if (int.Parse(tokens[0]) == id)
                {
                    Console.WriteLine($"\n Borrower having id {id} found!");
                    flag = true;
                    Console.Write("\n Enter new information in format {Name,Email}");
                    string email, name;
                    name = Console.ReadLine();
                    email = Console.ReadLine();
                    string _data = $"{id},{name},{email}";
                    sw.WriteLine(_data);
                }
                else
                {
                    sw.WriteLine(data);
                }
            }

            sr.Close(); sw.Close(); fin.Close(); fin2.Close();
            if (!flag)
            {
                Console.WriteLine($"Borrower having borrow id {id} not found!");
            }
            else
            {
                fin = new FileStream("Temp.txt", FileMode.Open, FileAccess.Read);
                fin2 = new FileStream("Borrowers.txt", FileMode.Create, FileAccess.Write);
                sr = new StreamReader(fin);
                sw = new StreamWriter(fin2);
                while ((data = sr.ReadLine()) != null)
                {
                    sw.WriteLine(data);
                }

                sr.Close(); sw.Close(); fin.Close(); fin2.Close();
            }
            Console.WriteLine($"\n Borrower having id {id} updated successfully!");
            File.Delete("Temp.txt");
        }

        // Works fine
        public void DeleteBorrower(int borrowerId) {

            FileStream fin = new FileStream("Borrowers.txt", FileMode.Open ,FileAccess.Read);
            StreamReader sr = new StreamReader(fin);

            FileStream fin2 = new FileStream("Temp.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fin2);

            string data;
            bool flag = false;
            while ((data = sr.ReadLine() ) != null)
            {
                var tokens = data.Split(',');
                if (int.Parse(tokens[0]) == borrowerId)
                {
                    // skipping copying it's data to Temp.txt for deletion purposes
                    flag = true;
                    continue;
                }
                else
                {
                    sw.WriteLine(data);
                }
            }

            sr.Close();sw.Close();
            fin.Close();
            fin2.Close();

            if(!flag)
            {
                Console.WriteLine($"\n Borrower having id {borrowerId} not found!");
                return;
            }

            fin = new FileStream("Borrowers.txt", FileMode.Create, FileAccess.Write);
            sw = new StreamWriter(fin);

            fin2 = new FileStream("Temp.txt", FileMode.Open, FileAccess.Read);
            sr = new StreamReader(fin2);

            while ((data = sr.ReadLine()) != null)
            {
                sw.WriteLine(data);
            }

            sr.Close(); sw.Close();
            fin.Close(); fin2.Close();

            File.Delete("Temp.txt");
            Console.WriteLine($"\n Borrower having id {borrowerId} deleted!");
        }

        // Book return issue
        public void RecordTransaction(Transaction transaction)
        {
            // First checking if the BorrowId is valid or not
            FileStream fin = new("Borrowers.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new(fin);

            string data;
            bool flag = false;

            while((data = sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');
                if (int.Parse(tokens[0]) == transaction.BorrowerId)
                {
                    flag = true; // means borrower is registered and valid
                }
            }

            sr.Close();
            fin.Close();

            if (flag == false)
            {
                Console.WriteLine($"\n Borrower having id {transaction.BorrowerId} is not a registered one!");
                return;
            }

            sr.Close();
            fin.Close();

            // Now checking if transaction id is unique or not
            fin = new("Transactions.txt", FileMode.Open, FileAccess.Read);
            sr = new(fin);
            while((data = sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');
                if (int.Parse(tokens[0]) == transaction.TransactionId)
                {
                    Console.WriteLine($"\n Transaction id {transaction.TransactionId} already exits!");
                    return;
                }
            }

            sr.Close();
            fin.Close();

            // Now checking if the transaction is done to borrow a book or return a book

            // Means user wants to borrow a book
            if (transaction.isBorrowed == true)
            {
                fin = new("Books.txt" , FileMode.Open , FileAccess.Read);
                sr = new(fin);

                flag = false;

                // checking whether the book that user wants to borrow is at the library or not
                while ((data = sr.ReadLine()) != null)
                {
                    var tokens = data.Split(',');
                    if (int.Parse(tokens[0]) == transaction.BookId && bool.Parse(tokens[4]) == false)
                    {
                        Console.WriteLine($"\n Book having id {transaction.BookId} is unavailable at the moment!");
                        return;
                    }
                    else if (int.Parse(tokens[0]) == transaction.BookId && bool.Parse(tokens[4]) == true)
                    {
                        flag = true;
                    }
                }

                sr.Close();
                fin.Close();

                if (flag == false)
                {
                    Console.WriteLine($"\n Book having id {transaction.BookId} not found!");
                    return;
                }

                // Now we have book available as well as valid borrowedId for transaction. So, we can record the transaction
                fin = new("Transactions.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new (fin);

                sw.Write($"\n{transaction.TransactionId},{transaction.BookId},{transaction.BorrowerId},{transaction.Date},{transaction.isBorrowed}");
                Console.WriteLine("\n Transaction data written successfully!");

                sw.Close();
                fin.Close();

                // Now changing the status of the book to make it unavailable as user has borrowed the book
                fin = new FileStream("Temp.txt", FileMode.Create, FileAccess.Write);
                FileStream fin2 = new FileStream("Books.txt", FileMode.Open, FileAccess.Read);
                sr = new(fin2);
                sw = new(fin);

                while( (data = sr.ReadLine()) != null)
                {
                    var tokens = data.Split(',');
                    if (int.Parse(tokens[0]) == transaction.BookId)
                    {
                        // means we have found the book, now we need to change the status
                        sw.WriteLine($"{tokens[0]},{tokens[1]},{tokens[2]},{tokens[3]},false");
                    }
                    else
                    {
                        sw.WriteLine(data);
                    }
                }

                sr.Close();
                sw.Close();
                fin.Close();
                fin2.Close();

                fin = new("Temp.txt", FileMode.Open, FileAccess.Read);
                fin2 = new("Books.txt", FileMode.Create, FileAccess.Write);
                sr = new(fin);
                sw = new(fin2);

                while( (data = sr.ReadLine()) !=null)
                {
                    sw.WriteLine(data);
                }

                sr.Close();
                sw.Close();
                fin.Close();
                fin2.Close();

                File.Delete("Temp.txt");
            }
            else
            {
                // Means user wants to return a book

                FileStream fin3 = new("Books.txt", FileMode.Open, FileAccess.Read);
                sr = new(fin3);

                flag = false;

                // checking whether the book that user wants to return is at the library or not
                while ((data = sr.ReadLine()) != null)
                {
                    var tokens = data.Split(',');
                    if (int.Parse(tokens[0]) == transaction.BookId && bool.Parse(tokens[4]) == true)
                    {
                        Console.WriteLine($"\n Book having id {transaction.BookId} is already available at the library!");
                        return;
                    }
                    else if (int.Parse(tokens[0]) == transaction.BookId && bool.Parse(tokens[4]) == false)
                    {
                        flag = true; // book id not found in library as it's status is also unavailable. So, the user can safely return it.
                    }
                }

                sr.Close();
                fin3.Close();

                if (!flag)
                {
                    Console.WriteLine("\n Book is not in the library!");
                    return;
                }

                // Now we need to check whether the right borrower is returning the book or not
                fin3 = new("Transactions.txt", FileMode.Open, FileAccess.Read);
                sr = new(fin3);

                while((data = sr.ReadLine()) != null)
                {
                    var tokens = data.Split(',');
                    if (int.Parse(tokens[1]) == transaction.BookId && int.Parse(tokens[2]) != transaction.BorrowerId)
                    {
                        Console.WriteLine($"\n Book having id {transaction.BookId} was not borrowed by borrower having id {transaction.BorrowerId}. So, he cannot return it!");
                        return;
                    }
                }

                sr.Close();
                fin3.Close();


                // Now we have book available as well as valid borrowedId for transaction. So, we can record the transaction
                fin3 = new("Transactions.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new(fin3);

                sw.Write($"\n{transaction.TransactionId},{transaction.BookId},{transaction.BorrowerId},{transaction.Date},{transaction.isBorrowed}");
                Console.WriteLine("\n Transaction data written successfully!");

                sw.Close();
                fin3.Close();

                fin3 = new("Temp.txt", FileMode.Create, FileAccess.Write);
                sw = new(fin3);

                FileStream fin2 = new("Books.txt", FileMode.Open, FileAccess.Read);
                sr = new(fin2);

                while ((data = sr.ReadLine()) != null)
                {
                    var tokens = data.Split(',');
                    if (int.Parse(tokens[0]) == transaction.BookId)
                    {
                        // means book has been found
                        sw.WriteLine($"{tokens[0]},{tokens[1]},{tokens[2]},{tokens[3]},true");
                    }
                    else
                    {
                        sw.WriteLine(data);
                    }
                }

                sr.Close();
                sw.Close();
                fin3.Close();
                fin2.Close();

                // Now copying data back to original file
                fin3 = new("Books.txt", FileMode.Create, FileAccess.Write);
                fin2 = new("Temp.txt", FileMode.Open, FileAccess.Read);

                sr = new(fin2);
                sw = new(fin3);
                while ((data = sr.ReadLine()) != null)
                {
                    sw.WriteLine(data);
                }

                sr.Close();
                sw.Close();
                fin3.Close();
                fin2.Close();
                File.Delete("Temp.txt");
            }
        }
        
        // Works fine
        public List<Book> GetBorrowedBooksByBorrower(int borrowerId)
        { 
            List<int> bookIds = new List<int>();
            List<Book> books = new List<Book>();

            FileStream fin = new FileStream("Borrowers.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new(fin);


            // First checking if borrower with borrow id 'borrowerId' exits or not
            bool flag = false;
            string data;
            while((data = sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');
                if (int.Parse(tokens[0]) == borrowerId)
                {
                    flag = true;
                    break;
                }
            }

            if(!flag)
            {
                Console.WriteLine($"\n Borrower with borrower id {borrowerId} is not registered!");
                return books;
            }

            sr.Close();
            fin.Close();

            fin = new("Transactions.txt", FileMode.Open, FileAccess.Read);
            sr = new(fin);

            while ((data = sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');
                if (int.Parse(tokens[2]) == borrowerId && bool.Parse(tokens[4]) == true)
                {
                    bookIds.Add(int.Parse(tokens[1]));
                }
            }

            sr.Close();
            fin.Close();

            fin = new("Books.txt", FileMode.Open, FileAccess.Read);
            sr = new StreamReader(fin);

            while((data = sr.ReadLine()) != null)
            {
                var tokens = data.Split(',');
                int id = int.Parse(tokens[0]);
                foreach (int i in bookIds)
                {
                    if ((id == i) && (bool.Parse(tokens[4]) == false)) // Means book id is in BookIds list and is un availble status in Books.txt
                    {
                        Book book = new Book
                        {
                            BookId = int.Parse(tokens[0]),
                            Title = tokens[1],
                            Author = tokens[2],
                            Genre = tokens[3],
                            isAvailable = false
                        };
                        books.Add(book); // adding book in list
                    }
                }
            }

            sr.Close();
            fin.Close();

            return books; // returning the list of books
        }
    }

    class Program
    {

        public static void Main()
        {
            Console.WriteLine("\n Welcome to library management sytem!");
            int opt;
            char doAgain;

            int id , trans_id , borrowedId;
            string title, author, genre, email , name,query;
            bool _isAvailable , _isBorrowed;
            List<Book> books = new List<Book>();

            Library library = new(); // instance of my library class
            do
            {
                library.displayMenu();

                opt = int.Parse(Console.ReadLine());
                switch (opt)
                {
                    case 1:
                        // User wants to add a book
                        Console.Write("\n Adding book!");
                        Console.Write("\n Write input details for book in order(id,title,author,genre,statusofbook(availble or not)) : ");
                        id = int.Parse(Console.ReadLine());
                        title = Console.ReadLine();
                        author = Console.ReadLine();
                        genre = Console.ReadLine();
                        _isAvailable = bool.Parse(Console.ReadLine());

                        // Creating book object
                        Book book = new Book
                        {
                            BookId = id,
                            Title = title,
                            Author = author,
                            Genre = genre,
                            isAvailable = _isAvailable
                        };

                        library.AddBook(book);
                        // Calling add book func
                        break;

                    case 2:
                        // user wants to remove a book
                        Console.WriteLine("\n Enter book id you want to delete from the library : ");
                        id = int.Parse(Console.ReadLine());

                        // calling book del func
                        library.RemoveBook(id);
                        break;

                    case 3:

                        // First displaying all the books info in library so user can choose which id numbered book to update
                        List<Book> allBooks = library.GetAllBooks();
                        Console.WriteLine("\n The books in the library are!");
                        foreach (Book tempBook in allBooks)
                        {
                            string data = $"{tempBook.BookId},{tempBook.Title},{tempBook.Author},{tempBook.Genre},{tempBook.isAvailable}";
                            Console.WriteLine(data);
                        }

                        // User wants to updata a book
                        Console.WriteLine("\n Enter book id you want to update : ");
                        id = int.Parse(Console.ReadLine());

                        // calling update book func
                        library.UpdateBook(id);
                        break;

                    case 4:
                        // User wants to register a new borrower
                        Console.Write("\n Enter borrower details in following format(id , name , email) :");
                        id = int.Parse(Console.ReadLine());
                        name = Console.ReadLine();
                        email = Console.ReadLine();

                        // Creating a borrower
                        Borrower borrower = new Borrower
                        {
                            BorrowerId = id,
                            Name = name,
                            Email = email
                        };

                        // calling func that registers borrower
                        library.RegisterBorrower(borrower);
                        break;

                    case 5:
                        // user wants to update a borrower

                        // First displaying all the borrowers info in library so user can choose which id numbered borrower to update
                        List<Borrower> allBorrowers = library.GetAllBorrowers();
                        Console.WriteLine("\n The registered borrowers are!");
                        foreach (Borrower tempBprrower in allBorrowers)
                        {
                            string data = $"{tempBprrower.BorrowerId},{tempBprrower.Name},{tempBprrower.Email}";
                            Console.WriteLine(data);
                        }

                        Console.WriteLine("\n Enter borrower id you want to update : ");
                        id = int.Parse(Console.ReadLine());

                        library.UpdateBorrower(id); // func call
                        break;

                    case 6:
                        // user wants to delete a borrower

                        // First displaying all the borrowers info in library so user can choose which id numbered borrower to delete
                        List<Borrower> _allBorrowers = library.GetAllBorrowers();
                        Console.WriteLine("\n The registered borrowers are!");
                        foreach (Borrower tempBprrower in _allBorrowers)
                        {
                            string data = $"{tempBprrower.BorrowerId},{tempBprrower.Name},{tempBprrower.Email}";
                            Console.WriteLine(data);
                        }

                        Console.Write("\n Enter id of borrower you want to delete : ");
                        id = int.Parse(Console.ReadLine());

                        // calling borrower del func
                        library.DeleteBorrower(id);
                        break;

                    case 7:
                        // user wants to borrow a book
                        Console.Write("\n Enter the inputs in following order(transaction id , bookid, borrowerid) : ");
                        trans_id = int.Parse(Console.ReadLine());
                        id = int.Parse(Console.ReadLine());
                        borrowedId = int.Parse(Console.ReadLine());
                        //_isBorrowed = bool.Parse(Console.ReadLine());

                        Transaction transaction = new Transaction
                        {
                            TransactionId = trans_id,
                            BookId = id,
                            BorrowerId = borrowedId,
                            Date = default,
                            isBorrowed = true
                        };

                        library.RecordTransaction(transaction);
                        break;

                    case 8:
                        // user wants to return a book
                        Console.Write("\n Enter the inputs in following order(transaction id , bookid, borrowerid) : ");
                        trans_id = int.Parse(Console.ReadLine());
                        id = int.Parse(Console.ReadLine());
                        borrowedId = int.Parse(Console.ReadLine());
                        //_isBorrowed = bool.Parse(Console.ReadLine());

                        Transaction _transaction = new Transaction
                        {
                            TransactionId = trans_id,
                            BookId = id,
                            BorrowerId = borrowedId,
                            Date = default,
                            isBorrowed = false
                        };

                        library.RecordTransaction(_transaction);
                        break;

                    case 9:
                        // user wants to search a book by title,author,genre
                        Console.Write("\n Enter title , author or genre to find co related books : ");
                        query = Console.ReadLine();

                        // calling search func
                        List<Book> searchedBooks = new List<Book>();
                        searchedBooks = library.SearchBook(query);

                        //displaying books
                        foreach (Book sbook in searchedBooks)
                        {
                            string data = $"{sbook.BookId},{sbook.Title},{sbook.Author},{sbook.Genre},{sbook.isAvailable}";
                            Console.WriteLine(data);
                        }
                        break;

                    case 10:
                        // user wants to view all book
                        List<Book> _allBooks = library.GetAllBooks();
                        Console.WriteLine("\n The books in the library are!");
                        foreach (Book tempBook in _allBooks)
                        {
                            string data = $"{tempBook.BookId},{tempBook.Title},{tempBook.Author},{tempBook.Genre},{tempBook.isAvailable}";
                            Console.WriteLine(data);
                        }
                        break;

                    case 11:
                        // user wants to view books borrowed by a specific user
                        Console.Write("\n Enter the borrower Id : ");
                        borrowedId = int.Parse(Console.ReadLine());

                        // func call
                        books = library.GetBorrowedBooksByBorrower(borrowedId);

                        foreach (Book b in books)
                        {
                            b.displayBookDetails();   
                        }
                        break;

                    case 12:
                        // user wants to exit the application
                        Console.Write("\n Closing the application!");
                        return;
                        break;

                    default:
                        Console.Write("\n Invalid option selected!");
                        break;
                }

                Console.Write("\n Run the menu Again?(Y|N) : ");
                doAgain = char.Parse(Console.ReadLine());

            } while (doAgain == 'Y' || doAgain == 'y');

            Console.WriteLine("\n Library management system closed successfully!");

        }
    }
}