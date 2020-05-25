using System;
using System.Linq;

namespace KK_Wardrobe
{
	public static partial class Strings
	{
		// Paths and Extensions
		public static string DATA_PATH = UserData.Path + KK_Wardrobe.PluginName;
		public static string CHARA_PATH = UserData.Path + "chara";

		public const string CARD_EXTENSION = ".png";


		// .bin Files
		public const string STATE_BIN = "state.bin";

		public const string CHECK_LIST_BIN = "checklist.bin";
		public const string CHECK_LIST_KKABMX_BIN = "checklist_kkabmx.bin";
		public const string CHECK_LIST_KSOX_BIN = "checklist_ksox.bin";
		public const string CHECK_LIST_KCOX_BIN = "checklist_kcox.bin";


		// Data keys for the wardrobes' `state.bin`.
		public const string STATE_WARDROBE_WEIGHT = "weight";
		public const string STATE_WARDROBE_ORDER = "order";
		public const string STATE_WARDROBE_NAME_MODE = "name_mode";
		public const string STATE_WARDROBE_NAME_LIST = "name_list";
		public const char STATE_WARDROBE_NAME_LIST_SEPARATOR = '\\';


		// Titles
		public const string WARDROBE_MANAGER = "Wardrobe Manager";


		// Dependent Strings
		public const string WARDROBE_WEIGHT = "Weight: {0} : {1:n2}%";
		public const string COUNT_CARDS = "{0} Card(s)";
		public const string WARDROBE_N = "Wardrobe {0}";
		public const string LAYER_N = "Layer {0}";
		public const string CHANCE = "{0:n2}%";
		public const string COUNT_LAYERS = "{0} Layer(s)";
		public const string EDIT_FOLDERNAME = "Edit `{0}`";
		public const string PASTE_CHECKLIST = "Paste `{0}`";
		public const string PASTE_CHECKLIST_OTHER = "Paste `{0} > {1}`";
		public const string FOLDER_ACTIVE_ERROR_DELETED = "{0}: {1} Active | {2} Error | {3} Deleted";
		public const string LOADED_FILENAME = "Loaded {0} `{1}`.";
		public const string DELETE_UNUSED = "Deleted unused {0} `{1}`.";
		public const string UNABLE_TO_SAVE = "Unable to save {0} with no name!";
		public const string CONTAINS_INVALID_CHAR = "{0} `{1}` contains an invalid file name character!";
		public const string CREATED_NEW_FOLDER = "Created new folder for {0} `{1}`.";
		public const string CARD_OVERWRITE = "Overwritten card `{0}`.";
		public const string CARD_SAVED = "Saved card `{0}`.";


		// Independent Strings
		public const string NEWLINE = "\n";
		public const string ARROW_RIGHT = ">";
		public const string ARROW_LEFT = "<";
		public const string T = "T";
		public const string ARROW_UP = "^";
		public const string ARROW_DOWN = "v";
		public const string B = "B";
		public const string X = "X";
		public const string PLUS = "+";
		public const string CLOSE = "Close";
		public const string CANCEL = "Cancel";
		public const string ADD_CARDS = "Add Card(s)";
		public const string NEW_WARDROBE = "New Wardrobe";
		public const string NEW_LAYER = "New Layer";
		public const string CARD_TYPE = "Card Type";
		public const string WARDROBE = "Wardrobe";
		public const string LAYER = "Layer";
		public const string CARD = "Card";
		public const string WARDROBES = "Wardrobes";
		public const string LAYERS = "Layers";
		public const string CARDS = "Cards";
		public const string OPEN = "Open";
		public const string EDIT = "Edit";
		public const string DELETE = "Delete";
		public const string RELOAD = "Reload";
		public const string SAVE = "Save";
		public const string DELETE_LAYER = "Delete Layer";
		public const string DELETE_WARDROBE = "Delete Wardrobe";
		public const string EDIT_CONFIGURATION = "Edit Configuration";
		public const string COPY = "Copy";
		public const string RESET = "Reset";
		public const string NAME = "Name";
		public const string NAME_MODE = "Name Mode";
		public const string WEIGHT = "Weight";
		public const string BROWSE = "Browse";
		public const string SELECT_CARDS = "Select card(s)";
		public const string CHARA_COORD_FILTER = "Character/Coordinate Cards (*.png)|*.png|All files|*.*";
		public const string CHARA_FILTER = "Character Cards (*.png)|*.png|All files|*.*";
		public const string LOADING = "Loading...";
		public const string LOADING_DONE = "Loading done!";
		public const string SAVING = "Saving...";
		public const string SAVING_DONE = "Saving done!";


		// Long Independent Strings/Arrays
		public static string[] WARDROBE_NAME_MODE = new string[]
		{
			"Blacklist",
			"Whitelist"
		};
		public const string WARDROBE_NAME_MODE_BLACKLIST =
			"Characters with these names CANNOT access this wardrobe.";
		public const string WARDROBE_NAME_MODE_WHITELIST =
			"Characters with these names CAN ONLY access this wardrobe.";
		public const string WARDROBE_NAME_MODE_TIP =
			"* [Last Name] [First Name]";
		public const string SAVE_CONFLICT =
			"There are wardrobes, layers, and/or cards " +
			"that are not visible in the wardrobe manager but are" +
			"present in the file explorer.\n" +
			"These will be permanently deleted when you save.";


		public static void Awake()
		{
			string[] coordinate = new string[]
			{
				"School",
				"Going Home",
				"Gym",
				"Swimsuit",
				"Club",
				"Casual",
				"Sleepwear"
			};

			WriteString
				("custom", "Body")
				("coordinate", "Clothes")
				("shapeValueBody", "Body Shape")
				("parts", "Clothing Parts", "clothes")
				("subPartsId", "Uniform Parts", "clothes")
				("KCOX", "KCOX{School01}", coordinate[0])
				("KCOX", "KCOX{School02}", coordinate[1])
				("KCOX", "KCOX{Gym}", coordinate[2])
				("KCOX", "KCOX{Swim}", coordinate[3])
				("KCOX", "KCOX{Club}", coordinate[4])
				("KCOX", "KCOX{Plain}", coordinate[5])
				("KCOX", "KCOX{Pajamas}", coordinate[6]);

			WriteString("coordinate", "coordinate", 0, coordinate);
			WriteString("KKABMXCoordinate", "KKABMXCoordinate", 0, coordinate);

			ChaFileDefine.BodyShapeIdx[] bodyShapeIdx =
				Enum.GetValues(typeof(ChaFileDefine.BodyShapeIdx)) as ChaFileDefine.BodyShapeIdx[];

			WriteString(
				"shapeValueBody", "shapeValueBody", 0,
				bodyShapeIdx.Select(v => v.ToString()).ToArray()
			);


			WriteString(
				"pupil", "pupil", 0,
				"Left Pupil",
				"Right Pupil"
			);

			WriteString(
				"parts", "parts", 0,
				"Top",
				"Bottom",
				"Bra",
				"Panties",
				"Gloves",
				"Pantyhose",
				"Legwear",
				"Indoor Footwear",
				"Outdoor Footwear"
			);

			WriteString(
				"subPartsId", "subPartsId", 0,
				"Shirt",
				"Jacket",
				"Decoration"
			);
		}
	}
}
