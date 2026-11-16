using Tradify.Core;

namespace Tradify.Seed;

// Synthetic data only. Names do not refer to real people.
public static class SeedData
{
    // Staff: U1 Alex Morgan, U2 Sam Lee, U3 Priya Shah (unassigned to J1 and J2)
    // Customers: C1 Chris Walker (J1), C2 Dana Price (J2)
    public static readonly IReadOnlyList<User> Users =
    [
        new User("U1", "alex", "Alex Morgan"),
        new User("U2", "sam", "Sam Lee"),
        new User("U3", "priya", "Priya Shah"),
        new User("U4", "chris", "Chris Walker"),
        new User("U5", "dana", "Dana Price"),
    ];

    // Jobs: J1 Kitchen refit, J2 Boiler service, J3 Roof repair (no assignments)
    public static readonly IReadOnlyList<Job> Jobs =
    [
        new Job("J1", "Kitchen refit"),
        new Job("J2", "Boiler service"),
        new Job("J3", "Roof repair"),
    ];

    public static readonly IReadOnlyList<(string UserId, string JobId)> Assignments =
    [
        ("U1", "J1"),
        ("U1", "J2"),
        ("U2", "J2"),
        ("U4", "J1"),
        ("U5", "J2"),
    ];

    private static readonly DateTimeOffset ThreadStart = new(2026, 10, 1, 9, 0, 0, TimeSpan.Zero);

    // Seed messages for J1.
    public static readonly IReadOnlyList<Message> J1Messages =
    [
        new Message(Guid.Parse("8c1e4f22-5b7a-4d19-9e03-6a2f0d7c1b41"), "J1", "U1", "Kitchen measurements are in the job notes.", ThreadStart),
        new Message(Guid.Parse("2d9a7e63-0f4c-4b8e-a1d5-93c6e8b20f7a"), "J1", "U4", "Thanks. Can you confirm the start date?", ThreadStart),
        new Message(Guid.Parse("f47b1c05-9e2d-4a36-8c71-0b5d3e9a6f28"), "J1", "U1", "Starting Monday 13 October.", ThreadStart),
        new Message(Guid.Parse("5a0e8d94-3c6f-4f21-b7e9-d2a1c4f05e83"), "J1", "U4", "Great, I'll arrange access.", ThreadStart),
    ];

    public static void ApplyMessages(IMessageRepository messages)
    {
        foreach (var message in J1Messages)
        {
            messages.Add(message);
        }
    }
}
