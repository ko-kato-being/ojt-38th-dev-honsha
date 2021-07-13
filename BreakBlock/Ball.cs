﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace BreakBlock
{
    class Ball
    {
        public int pitch;     //移動の割合
        private int hitNum ;  //跳ね返り回数の変数宣言

        private PictureBox pictureBox;   //描画するpictureBox
        private Bitmap canvas;          //描画するキャンバス
        public int positionX;          //横位置(x座標)
        public int positionY;          //縦位置(y座標)
        private int previousX;          //以前の横位置(x座標)
        private int previousY;          //以前の縦位置(y座標)
        public int directionX;         //移動方向(x座標)(+1 or -1)
        public int directionY;         //移動方向(y座標)(+1 or -1)
        public int radius;             //円の半径
        private Brush brushColor;       //塗りつぶす色

        public int score = 0;
        public int finish = 0;         //0のときまだプレイ中、1のときクリア、2のときゲームオーバー



        //ボールコンストラクタ    
        public Ball(PictureBox pb, Bitmap cv, Brush cl)
        {
            pictureBox = pb;            //描画するpictureBox
            canvas = cv;                //描画するキャンバス
            brushColor = cl;            //塗りつぶす色

            radius = 10;                //円の半径の初期設定
            pitch = radius / 2;         //移動の割合の初期設定(半径の半分)

            //ToDo発射角度の決定
            Random r = new Random();
            int randomX = r.Next(0, 100) % 2;
            if (randomX == 0)
            {
                directionX = -1;
                directionY = -1;
            }
            else
            {
                directionX = +1;
                directionY = -1;
            }

        }

        //指定した位置にボールを描く
        public void PutCircle(int x, int y)
        {
            //現在の位置を記憶
            positionX = x;
            positionY = y;

            using (Graphics g = Graphics.FromImage(canvas))
            {
                //弾をbrushColorで指定された色で描く
                g.FillEllipse(brushColor, x - radius, y - radius, radius * 2, radius * 2);

                pictureBox.Image = canvas;
            }
        }

        //指定した位置のボールを消す(黒で描く)
        public void DeleteCircle()
        {
        
                //初めて呼ばれて以前の値が無い時
                if (previousX == 0)
                {
                    previousX = positionX;
                }
                if (previousY == 0)
                {
                    previousY = positionY;
                }

                using (Graphics g = Graphics.FromImage(canvas))
                {
                    //弾を黒で描く
                    g.FillEllipse(Brushes.Black, previousX - radius, previousY - radius, radius * 2, radius * 2);

                    pictureBox.Image = canvas;
                }
            
        }

        //ボールを動かす
        public void Move(List<Block> blocks)
        {

            //以前の表示を削除
            DeleteCircle();

            //新しい移動先の計算
            int x = positionX + pitch * directionX;
            int y = positionY + pitch * directionY;


            //壁で跳ね返る補正
            if (x >= canvas.Width - radius) //右端に来た場合の判定
            {
                directionX = -1;
            }
            if (x <= radius) //左端に来た場合の判定
            {
                directionX = +1;
            }
            if (y <= radius) //上端に来た場合の判定
            {
                directionY = +1;
            }

            //ボールがブロックに当たった時の跳ね返り処理
            //ブロックを消す処理
            for (int i = 0; i < blocks.Count; i++)
            {
                if ((y >= blocks[i].top - radius) && (y <= blocks[i].bottom + radius) && (x >= blocks[i].left - radius) && (x <= blocks[i].right + radius))
                {
                    Acceleration();
                    //下辺の処理
                    if (directionY == -1)
                    {
                        directionY *= -1;
                        blocks[i].DeleteBlock();
                        blocks.RemoveAt(i);
                        score += 10;
                        continue;
                    }
                    //左辺の処理
                    if (directionX == 1)
                    {
                        directionX *= -1;
                        blocks[i].DeleteBlock();
                        blocks.RemoveAt(i);
                        score += 10;
                        continue;
                    }
                    //右辺の処理
                    if (directionX == -1)
                    {
                        directionX *= -1;
                        blocks[i].DeleteBlock();
                        blocks.RemoveAt(i);
                        score += 10;
                        continue;
                    }
                    //上辺の処理
                    if (directionY == 1)
                    {
                        directionY *= -1;
                        blocks[i].DeleteBlock();
                        blocks.RemoveAt(i);
                        score += 10;
                        continue;
                    }
                }
            }
       
            //バーに衝突するとで跳ね返る
            if ((x >= Bar.barpositionX) && (x <= Bar.barpositionX + 90) && (y >= (350 - radius)) && (y <= 350 + radius))
            {
                directionY = -1;
            }

            //跳ね返り補正fを反映した値で新しい位置を計算
            positionX = x + directionX;
            positionY = y + directionY;

            //新しい位置に描画
            PutCircle(positionX, positionY);

            //新しい位置を以前の値として記憶
            previousX = positionX;
            previousY = positionY;


            //下端に来たときゲームオーバー画面に移る
            if (y >= canvas.Height)
            {
                finish = 2;
            }

            //跳ね返り補正fを反映した値で新しい位置を計算
            positionX = x + directionX;
            positionY = y + directionY;

            //新しい位置に描画
            PutCircle(positionX, positionY);

            //新しい位置を以前の値として記憶
            previousX = positionX;
            previousY = positionY;

        }
        private void Acceleration()
        {
            hitNum += 1;
            if (hitNum == 2)
            {
                pitch += 1;
                hitNum = 0;
            }
        }
    }

}


