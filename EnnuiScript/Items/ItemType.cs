namespace EnnuiScript.Items
{
	public enum ItemType
	{
		None, // null
		Any, //  anything, null or non null
		Something, // any non null

		Type,

		Number,
		String,
		Bool,

		Symbol,
		Space,
		List,
		Invokeable
	}
}