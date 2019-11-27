using System;
using System.Collections.Generic;

namespace Statecharts.NET.SCXML
{
    abstract class BaseContainer { }
    class Container<T> : BaseContainer
    {
        public T Data { get; set; }
    }

    abstract class Animal { }

    class Dog : Animal
    {
        public int Legs { get; set; }
    }
    
    class Cat : Animal
    {
        public string Color { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BaseContainer container = new Container<Animal>() { Data = new Dog() { Legs = 4 } };

            switch(container)
            {
                case Container<Cat> _: Console.WriteLine("I am a Cat Container"); break;
                case Container<object> _: Console.WriteLine("I am an object Container"); break;
                case Container<Animal> animalContainer:
                    Console.WriteLine("I am an Animal Container");
                    switch(animalContainer.Data)
                    {
                        case Dog dog: Console.WriteLine("I am a Dog"); break;
                        case object _: Console.WriteLine("I am an object"); break;
                    }
                    break;
                case Container<Dog> _: Console.WriteLine("I am a Dog Container"); break;
            }
        }
    }
}
