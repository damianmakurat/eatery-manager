   M S B A M L   `   `   `  ����I  Eeatery-manager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:eatery_manager.Windowseatery_manager.Windows  5   	   )    #eatery_manager.Windows.TablesWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6local$clr-namespace:eatery_manager.Windows     5   	     � Title$  TablesWindow��5	   	   $	��450��6   $	��800��6+   .��5
      � $��10q�6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ TablesDataGrid��5      $��0��5         AutoGenerateColumns$ False��5         CanUserAddRows$ False��5      ��5      �� *Tables+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      
IsReadOnly$
 True��6M      Width5  /System.Windows.Controls.DataGridLengthConverter$ 50 6_   1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 61   �� *	TableId+6_     5      $ 
Location X��6%   $ * 6W   	 69   �� *	LocationX+6W     5      $ 
Location Y��6%   $ * 6W   	 69   �� *	LocationY+6W     5      $ Capacity��6%   $	 100 6T   	 67   �� *
Capacity+6T   5      �� 5   
   $��1��6   $��0,10,0,0q�6"   
 �� Orientation$
 
Horizontal=�64   $��Center=�6M   $
��Left=�6h   .+�5!      �� -   5         ��Name$ TbLocationX��5!      $	��100��6+   $��0,0,10,0q�67   $��
Location X��6I   
�������6[   �� 5"      -   5        $ TbLocationY��5"      $	��100��6+   $��0,0,10,0q�67   $��
Location Y��6I   
�������6[   �� 5#      -   5        $ 
TbCapacity��5#      $��60��6*   $��0,0,10,0q�65   $��Capacity��6G   
�������6W   �� 5%      -   5        $��Dodaj stolik��5%      $	��100��6,   68   5&      5'      5(      