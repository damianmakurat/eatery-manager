   M S B A M L   `   `   `  ����  eatery-manager'    !eatery_manager.Windows.MenuWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  
MenuWindow��5   	   $	��450��6   $	��800��6)   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5	      $
��Auto �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ MenuDataGrid��5      $��0��6)      AutoGenerateColumns$ False��66      
IsReadOnly$
 True��6R   $��0,0,0,10q�5   -   ��6   �� *	MenuItems+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Nazwa��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6M   1  +System.Windows.Controls.DataGridBoundColumn   Binding 64   �� *Name+6M     5      $ 	Kategoria��6%   $	 150 6U    68   �� *
Category+6U     5      $
 Cena��6%   $	 100 6]    63   �� *Price+	 �� StringFormat$	 C��6]     5      $ 
Składniki��6%   $	 250 6X    68   �� *Ingredients+6X   5      �� 5   
   $��1��6   
 �� Orientation$
 
Horizontal=�6"   $
��Left=�6;   $��Center=�6V   .+�5      �� -   5         ��Name$ TbName��5      $	��150��6&   $��0,0,5,0q�62   $��Nazwa��6C   �� 5      -   5        $ 
TbCategory��5      $	��120��6*   $��0,0,5,0q�66   $��	Kategoria��6G   �� 5      -   5        $ TbPrice��5      $��80��6'   $��0,0,5,0q�62   $
��Cena��6C   �� 5      -   5        $ TbIngredients��5      $	��250��6-   $��0,0,5,0q�69   $��
Składniki��6J   �� 5      -   5         ��Name$ BtnAdd��5      $��Dodaj��6%   $��80��65   6@   5       5!      5"      