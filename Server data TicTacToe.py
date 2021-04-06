import socket
from os import system
import _thread
import sqlite3
import time
import datetime


def update_score(id):
    """
    Add point to the player how won in the game. need to connect again to the data base because is a new thread.
    :param id: id of the player that won
    :return:
    """
    global c2
    conn2 = sqlite3.connect("TicTacToeData.db")
    c2 = conn2.cursor()
    create_table(c2, False)
    c2.execute("SELECT score FROM usersDetails WHERE id = (?)", [id])
    score = c2.fetchall()
    print(score)
    new_score = int(float(str(score[0][0]))) + 1
    c2.execute("UPDATE usersDetails SET score = (?) WHERE id = (?)", (new_score, id,))
    conn2.commit()
    c2.close()
    conn2.close()


def recv_from_player(player1_sock, player2_sock, id1, id2):
    """
    waiting for data from the player and send to the other.
    :param player1_sock: socket to the first player
    :param player2_sock: socket to the second player
    :param id1: id of the first player
    :param id2: id of the second player.
    :return:
    """
    game_over = False
    while not game_over:
        data = player1_sock.recv(64).decode()
        if data == "exit":
            to_send = "You won automatically"
            player2_sock.send(to_send.encode())
            update_score(id2)
            break
        try:
            data = data.split("-")
            move = data[0]
            is_game_over = data[1]
        except:
            continue
        to_send = "{}-{}-{}".format(move, "True", game_over)
        if is_game_over == "notEnded":
            player2_sock.send(to_send.encode())
        if is_game_over != "notEnded":
            game_over = True
            to_send = "{}-{}-{}".format(move, "True", game_over)
            player2_sock.send(to_send.encode())
            update_score(id1)


def check_if_exist(user_name, password, new):
    """
    Check if the user already exist
    :param user_name: the user name
    :param password: the password
    :param new: if new player
    :return:
    """
    i = 0
    c.execute("SELECT username, password FROM usersDetails")
    for row in c.fetchall():
        i = i+1
        if new:
            if row[0] == user_name:
                return True
        else:
            if row[0] == user_name:
                if row[1] == password:
                    return True
    return False


def add_new_user(user_name, password, client_sock):
    """
    Add a new user to the database file
    :param user_name: the user name
    :param password: the password
    :param client_sock: the client socket
    :return:
    """
    if check_if_exist(user_name, password, True):
        to_send = "The name already exist"
    else:
        c.execute("SELECT * FROM usersDetails")
        id = len(c.fetchall())
        unix = time.time()
        time_registered = str(datetime.datetime.fromtimestamp(unix).strftime("%Y-%m-%d %H:%M:%S"))
        to_send = "Ok," + str(id)
        c.execute("INSERT INTO usersDetails (id, username, password, time, score) VALUES (?,?,?,?,?)",
                  (id, user_name, password, time_registered, 0))
        conn.commit()
    client_sock.send(to_send.encode())


def check_if_correct(user_name, password, client_sock):
    """
    check if the detaits of the player are correct
    :param user_name:
    :param password:
    :param client_sock:
    :return:
    """
    if check_if_exist(user_name, password, False):
        to_send = "Ok"
    else:
        to_send = "One or more of the details you entered are incorrect"
    client_sock.send(to_send.encode())


def want_results():
    """
    celect the five best player
    :return: the best five player
    """
    c.execute("SELECT * from usersDetails ORDER BY score")
    return (c.fetchall()[-1:-6:-1])


def find_id(user_name):
    """
    find the user id according to his user name
    :param user_name: user name
    :return: user id
    """
    c.execute("SELECT username from usersDetails")
    for name in c.fetchall():
        if str(name[0]) == user_name:
            c.execute("SELECT id FROM usersDetails WHERE username = (?)", name)
            id1 = c.fetchall()
            return int(float(str(id1[0][0])))


def waiting_for_connection():
    """
    waiting for connection of new users and do what they ask for.
    :return:
    """
    how_many_user = 0
    while True:
        client_sock, addr = sock.accept()  # blocking method
        data = client_sock.recv(64).decode()
        data = data.split("-")
        if data[0] == "two players": #אם המידע במקום ה 0 שווה לזה
            how_many_user = how_many_user + 1
            if how_many_user % 2 != 0:
                client_sock1, username1 = client_sock, data[1]
                id1 = find_id(username1)
            else:
                username2 = data[1]
                id2 = find_id(username2)
                how_many_user = 0
                client_sock1.send("True".encode())
                client_sock.send("False".encode())
                _thread.start_new_thread(recv_from_player, (client_sock1, client_sock, id1, id2,))
                _thread.start_new_thread(recv_from_player, (client_sock, client_sock1, id2, id1,))
        elif data[0] == "Want results":
            list_of_player = want_results()
            client_sock.send(str(list_of_player).encode())
        else:
            if data[0] != "False":  # אם המשתמש הוא משתמש חדש שנרשם
                add_new_user(data[1], data[2], client_sock)
            else:
                check_if_correct(data[1], data[2], client_sock)


def create_table(c, main):
    """
    if the table doesnt exist create a new one
    :param c:
    :param main:
    :return:
    """
    global c2
    if (main):
        c.execute("CREATE TABLE IF NOT EXISTS usersDetails(id REAL, username TEXT, password TEXT, time TEXT, score REAL)")
    else:
        c2.execute("CREATE TABLE IF NOT EXISTS usersDetails(id REAL, username TEXT, password TEXT, time TEXT, score REAL)")


if __name__ == '__main__':
    system("title Server")
    HOST = "127.0.0.1"
    PORT = 7000
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.bind((HOST, PORT))
    sock.listen(2)
    conn = sqlite3.connect("TicTacToeData.db")
    c = conn.cursor()
    create_table(c, True)
    waiting_for_connection()
    c.close()
    conn.close()
