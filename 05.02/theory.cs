System.Threading.Tasks

Task task = Task.Run(() => {
    
});
/*
свойство Status - отображает текущее состояние
    *Created создана, но еще не запущена
    *Running выполняется
    *Comleted заверешна успешно
    *Canceled отменена
    *Faulted ошибка в процессе
 */
Wait()

await Task.Run(() =>
{

});

Task.WhenAll() - ожидидает всех
Task.WhenAny() - возврщает первую завершенную

