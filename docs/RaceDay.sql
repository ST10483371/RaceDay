Create database RaceDay;
Use RaceDay;

CREATE TABLE Users(
UserID int IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(50) NOT NULL,
Email VARCHAR(20) UNIQUE NOT NULL,
Password VARCHAR(20) NOT NULL,
ContactInfo VARCHAR(10),
UserType VARCHAR(50) NOT NULL,
CHECK (UserType IN ('Participant', 'Organizer'))
)
