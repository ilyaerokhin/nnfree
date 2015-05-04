using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Free.nn
{
    class NNFreeAPI
    {
        private SocketClient client;
        private int PORT;
        private string HOST = null;
        private string result = null;
        private bool IsConnected = false;

        // конструктор
        public NNFreeAPI(string host,int port)
        {
            this.HOST = host;
            this.PORT = port;
            client = new SocketClient();
        }
        // подключение
        public int Connect()
        {
            this.result = this.client.Connect(this.HOST, this.PORT);

            if (!this.result.Contains("Success"))
            {
                this.IsConnected = false;
                return -1; // неудача
            }
            else
            {
                this.IsConnected = true;
                return 0; // удача
            }
        }
        // закрываем соединение
        public void Close()
        {
            this.client.Close();
            this.IsConnected = false;
        }
        // получить id приложения Вконтакте
        public int App_ID()
        {
            if(this.IsConnected)
            {
                this.client.Send("<get_app_id>");
                this.result = this.client.Receive();
                if(this.result.Contains("app_id="))
                {
                    return Int32.Parse(this.result.Split(new Char[] { '=','>' })[1]);
                }
                else
                {
                    return -2; // ошибка в запросе
                }
            }
            else
            {
                return -1; // нет соединения
            }
        }
        // проверка пользователя
        public int CheckUser(string id, string name)
        {
            if (this.IsConnected)
            {
                this.client.Send("<check_user/" + id + "/" + name + ">");
                this.result = this.client.Receive();

                if (this.result.Contains("<old_user>"))
                {
                    return 0; // старый пользователья
                }
                else
                {
                    if (this.result.Contains("<new_user>"))
                    {
                        return 1; // новый пользователья
                    }
                    else
                    {
                        return -2; // неизвестная ошибка
                    }
                }
            }
            else
            {
                return -1; // нет соединения
            }
        }
        // Добавление нового объявления
        public int AddAdvert(string id, string low_text, string big_text)
        {
            if (this.IsConnected)
            {
                this.client.Send("<add_advert/" + id + "/" + low_text + "/" + big_text + ">");
                this.result = this.client.Receive();

                if (this.result.Contains("<add_advert/"))
                {
                    if (this.result.Contains("<add_advert/bad>"))
                    {
                        return -3; // ошибка в запросе
                    }
                    else
                    {
                        return Int32.Parse(this.result.Split(new Char[] { '/', '>' })[1]); // возвращаем id объявления
                    }
                }
                else
                {
                    return -2; // неизвестная ошибка
                }
            }
            else
            {
                return -1; // отсутствует подключение
            }
        }
        // Получить объявление по id
        public string GetAdvert(string advert_id)
        {
            if (this.IsConnected)
            {
                this.client.Send("<get_advert/" + advert_id + ">");
                this.result = this.client.Receive();

                if (this.result.Contains("<get_advert/"))
                {
                    if (this.result.Contains("<get_advert/bad>"))
                    {
                        return null; // ошибка в запросе
                    }
                    else
                    {
                        return this.result.Split(new Char[] { '/', '>' })[1] // owner_id
                               + "/"
                               + this.result.Split(new Char[] { '/', '>' })[2] // low_text
                               + "/"
                               + this.result.Split(new Char[] { '/', '>' })[3] // big_text
                               +"/"
                               + this.result.Split(new Char[] { '/', '>' })[4]; // name
                    }
                }
                else
                {
                    return null; // неизвестная ошибка
                }
            }
            else
            {
                return null; // отсутствует подключение
            }
        }
        // получить 30 объявлений, последним из которых будет
        // объявление с id, принимаемым параметром метода
        public string ListAds(string advert_id)
        {
            if (this.IsConnected)
            {
                this.client.Send("<list_adverts/" + advert_id + ">");
                this.result = this.client.Receive();

                if (this.result.Contains("<list_adverts/"))
                {
                    string[] ads = result.Split(new Char[] { '/', '<', '>' });

                    // флажок нужен для пропуска первого захода
                    bool flag = true;
                    string str = "";

                    // в цикле заполняем список объявлений
                    foreach (string s in ads)
                    {
                        // если строка не пустая
                        if (s.Trim() != "")
                        {
                            // исключаем первый заход
                            if (flag != true)
                            {
                                str = str + s + "/";
                            }
                            else
                            {
                                // смена флага
                                flag = false;
                            }
                        }
                    }
                    return str; // ad1/ad2/../adn/
                }
                else
                {
                    return null; // неизвестная ошибка
                }
            }
            else
            {
                return null; // отсутствует подключение
            }
        }
        // получить количество объвлений 
        // (или id последнего объявления)
        public int NumberOfAds()
        {
            if (this.IsConnected)
            {
                this.client.Send("<number_of_ads>");
                this.result = this.client.Receive();

                if (this.result.Contains("number_of_ads="))
                {
                    return Int32.Parse(this.result.Split(new Char[] { '=', '>' })[1]); // количество объявлений в системе
                }
                else
                {
                    return -2; // ошибка запроса
                }
            }
            else
            {
                return -1; // нет подключения
            }
        }
        // удалить объявление
        public int DeleteAdvert(string user_id, string advert_id)
        {
            if (this.IsConnected)
            {
                this.client.Send("<delete_advert/" + user_id + "/" + advert_id + ">");
                this.result = this.client.Receive();

                if(this.result.Contains("<delete_advert/ok>"))
                {
                    return 0; // успешно
                }
                else
                {
                    if(this.result.Contains("<delete_advert/bad>"))
                    {
                        return -2; // ответ о неудаче
                    } 
                    else
                    {
                        return -3; // неизвестная ошибка
                    }
                }
            }
            else
            {
                return -1; // нет соединения
            }
        }
        // пожаловаться на объявление
        public int BadAdvert(string user_id, string advert_id)
        {
            if (this.IsConnected)
            {
                this.client.Send("<bad_advert/" + user_id + "/" + advert_id + ">");
                this.result = this.client.Receive();

                if(this.result.Contains("<bad_advert/ok>"))
                {
                    return 0; // успех
                }
                else
                {
                    return -2; // неизвестная ошибка
                }
            }
            else
            {
                return -1; // нет соединения
            }
        }
        // количество объвлений пользователя
        public int NumberOfUserAds(string user_id)
        {
            if (this.IsConnected)
            {
                this.client.Send("<number_of_user_ads/" + user_id + ">");
                this.result = this.client.Receive();
                
                if(this.result.Contains("<number_of_user_ads/"))
                {
                    if(this.result.Contains("<number_of_user_ads/bad>"))
                    {
                        return -3; // ошибка запроса
                    }
                    else
                    {
                        return Int32.Parse(this.result.Split(new Char[] { '/', '>' })[1]); // возвращаем количество объвлений
                    }
                }
                else
                {
                    return -2; // неизвестная ошибка
                }
            }
            else
            {
                return -1; // нет соединения
            }
        }
        // получить 30 объявлений пользователя, последним из которых будет
        // объявление с id, принимаемым параметром метода
        public string ListUserAds(string user_id, string number_of_ad)
        {
            if (this.IsConnected)
            {
                this.client.Send("<list_user_ads/" + user_id + "/" + number_of_ad + ">");
                this.result = this.client.Receive();

                if (this.result.Contains("<list_user_ads/"))
                {
                    string[] ads = result.Split(new Char[] { '/', '<', '>' });

                    // флажок нужен для пропуска первого захода
                    bool flag = true;
                    string str = "";

                    // в цикле заполняем список объявлений
                    foreach (string s in ads)
                    {
                        // если строка не пустая
                        if (s.Trim() != "")
                        {
                            // исключаем первый заход
                            if (flag != true)
                            {
                                str = str + s + "/";
                            }
                            else
                            {
                                // смена флага
                                flag = false;
                            }
                        }
                    }
                    return str; // #|ad1/#|ad2/../#|adn/
                }
                else
                {
                    return null; // неизвестная ошибка
                }
            }
            else
            {
                return null; // отсутствует подключение
            }
        }
    }
}
