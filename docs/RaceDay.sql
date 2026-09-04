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

CREATE TABLE Participants(
ParticipantId int IDENTITY(1,1) PRIMARY KEY,
DateOfBirth VARCHAR(20) NOT NULL,
Gender VARCHAR(20) NOT NULL,
EmergencyContact VARCHAR(20) 
TeamName VARCHAR(20)
)

CREATE TABLE Organizers(
OrganizerID int IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(20) UNIQUE NOT NULL,
Contact_Number(10) NOT NULL
)

