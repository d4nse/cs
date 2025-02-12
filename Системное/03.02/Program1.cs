unsafe
{
    int number = 10; // обычная переменная
    int* pointer = &number; // указатель, который хранит адрес переменной number

    Console.WriteLine("Значение number: " + number); // 10
    Console.WriteLine("Адрес number: " + (int)pointer); // адрес переменной

    // разыменование указателя для получения значения
    Console.WriteLine("Значение через указатель: " + *pointer); // Вывод: 10

    *pointer = 20; // изменяем значение переменной number через указатель

    Console.WriteLine("Новое значение number: " + number); // Вывод: 20
}