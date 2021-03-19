public interface InventoryUI
{
    public void SetItem(int slot, IItem item);
    public IItem GetItem(int slot);
    public IItem GetSelectedItem();
    public int GetSelectedIndex();
    public void Select(int slot);
    public int GetSize();
}
