import tkinter as tk
import random

class BrickBreaker:
    def __init__(self, root):
        self.root = root
        self.root.title("Python Brick Breaker")
        
        # 게임 설정
        self.width = 600
        self.height = 400
        self.canvas = tk.Canvas(self.root, width=self.width, height=self.height, bg="black")
        self.canvas.pack()

        # 패들 설정
        self.paddle = self.canvas.create_rectangle(250, 370, 350, 385, fill="white")
        self.paddle_speed = 20

        # 공 설정
        self.ball = self.canvas.create_oval(290, 200, 310, 220, fill="yellow")
        self.ball_dx = 3
        self.ball_dy = -3

        # 벽돌 생성 (줄마다 다른 색상)
        self.bricks = []
        #         self.colors = ["#FF5555", "#FFB86C", "#F1FA8C", "#50FA7B", "#8BE9FD"] # 빨, 주, 노, 초, 파
        self.create_bricks()

        # 키 바인딩
        self.root.bind("<Left>", self.move_left)
        self.root.bind("<Right>", self.move_right)

        self.game_loop()

    def create_bricks(self):
        rows = 5
        cols = 8
        brick_width = 70
        brick_height = 20
        padding = 5
        offset_x = 10
        offset_y = 30

        for r in range(rows):
            color = self.colors[r % len(self.colors)] # 줄마다 색상 변경
            for c in range(cols):
                x1 = offset_x + c * (brick_width + padding)
                y1 = offset_y + r * (brick_height + padding)
                x2 = x1 + brick_width
                y2 = y1 + brick_height
                brick = self.canvas.create_rectangle(x1, y1, x2, y2, fill=color, outline="black")
                self.bricks.append(brick)

    def move_left(self, event):
        if self.canvas.coords(self.paddle)[0] > 0:
            self.canvas.move(self.paddle, -self.paddle_speed, 0)

    def move_right(self, event):
        if self.canvas.coords(self.paddle)[2] < self.width:
            self.canvas.move(self.paddle, self.paddle_speed, 0)

    def game_loop(self):
        # 공 이동
        self.canvas.move(self.ball, self.ball_dx, self.ball_dy)
        pos = self.canvas.coords(self.ball)

        # 벽 충돌 감지
        if pos[0] <= 0 or pos[2] >= self.width:
            self.ball_dx *= -1
        if pos[1] <= 0:
            self.ball_dy *= -1
        
        # 패들 충돌 감지
        paddle_pos = self.canvas.coords(self.paddle)
        if pos[2] >= paddle_pos[0] and pos[0] <= paddle_pos[2]:
            if pos[3] >= paddle_pos[1] and pos[3] <= paddle_pos[3]:
                self.ball_dy *= -1

        # 벽돌 충돌 감지
        for brick in self.bricks[:]:
            brick_pos = self.canvas.coords(brick)
            if pos[2] >= brick_pos[0] and pos[0] <= brick_pos[2]:
                if pos[3] >= brick_pos[1] and pos[1] <= brick_pos[3]:
                    self.canvas.delete(brick)
                    self.bricks.remove(brick)
                    self.ball_dy *= -1
                    break

        # 게임 오버 (바닥에 닿았을 때)
        if pos[3] >= self.height:
            self.canvas.create_text(300, 200, text="GAME OVER", fill="white", font=("Arial", 30))
            return

        # 승리 조건
        if not self.bricks:
            self.canvas.create_text(300, 200, text="YOU WIN!", fill="white", font=("Arial", 30))
            return

        self.root.after(10, self.game_loop)

if __name__ == "__main__":
    root = tk.Tk()
    game = BrickBreaker(root)
    root.mainloop()