INCLUDE Functions.ink

VAR swimmerCamOn=false
VAR loadCutscene=false
VAR intensity=0
VAR showcaseMode=0
VAR showcaseProgress=0

//Include: thoughts about evolution, heartbreak, establish evolution
// maybe something like... i've noticed this thing inside of me 
// or sth growing out of me 

=== intro01 ===
~ stopSinging()
~ forceTutorialPart(3)
Ex: So, \\pauseI'm leaving. # time: 3
~ intensity=1
~ pause(2)
MC: What? # time: 3
~ continueSinging()
~ nextBrain()
~ forceTutorialPart(2)
-> END

=== intro02 ===
~ stopSinging()
~ forceTutorialPart(3)
Where are you going? # time: 3
Ex: To the surface. # time: 3
MC: I.. # time: 3
Can I come with you? # time: 3
~ forceTutorialPart(2)
~ continueSinging()
~ nextBrain()
-> END

// Main character is swimming in the dark. As the conversation progresses, they begin to see two sexual organs entangling. They have the faces of the Protagonist and the Ex as heads. The faces are obscured until the end of the conversation.
=== intro1 ===
~ stopSinging()
~ forceTutorialPart(3)
MC: I said can I come with you?
~ loadInt("showcaseMode")
{ showcaseMode==1:
    Ex: I don't think you would survive it.  # time: 3
-else:
    Ex: That wouldn't be right for you.  # time: 3
}
~ pause(2)
MC: How do you know that? And, why are you going? Is it something I did? # time: 5
Ex: No. \\pauseThere's just the calling. # time: 5
~pause(2)
~ loadInt("showcaseMode")
{ showcaseMode==0:
    MC: Will you come back? Every now and then? # time:3
    MC: Or maybe we can still communicate, with telepathy? # time: 3
    Ex: ..It probably doesn't work that way. # time: 3
    ~pause(4)
}
Ex: Anyway..
~nextBrain()
I will get going.
~ pause(4)
MC: Hold on..
~ swimmerCamOn=true
~ changeDialogueView(2)
MC: Don't leave me behind! #speed: fast
~ changeDesire("<br>Follow <br>them.")
~ pauseTutorial(false)
~ finishTutorialPart(3)
-> END

=== intro2 ===
# ambient
~ changeDialogueView(2)
~ intensity=2
~ pause(1)
MC: Are... are you still there? Do you receive me? # time:5
Ex: What is it? # time:3
~ loadInt("showcaseMode")
{ showcaseMode==1:
    ~showcaseProgress=1
}
MC: What does this mean for us..? # time: 5 # speed: slow
Ex: Evidently, this has to be our end. # time: 3
MC: And are you sure I can't -- # time: 1.5 # speed: fast
Ex: I don't think you would survive it. # time: 3
-> END

=== intro3 ===
# ambient
~ intensity=3
~ changeDialogueView(2)
MC: Did you even like me at all? Or was this just... some sort of a.. a distraction? Just to pass the time?! # time: 7
Ex: Sure, I did. \\pause\\pauseThis has nothing to do with you. # time: 6
~ loadInt("showcaseMode")
{ showcaseMode==1:
    ~showcaseProgress=2
}
-> END

=== intro4 ===
# ambient
~ intensity=4
~ changeDialogueView(2)
MC: Will you miss me at all? # time: 3
~ loadInt("showcaseMode")
{ showcaseMode==1:
    Ex: ... # time:3 #speed:slow
    ~showcaseProgress=3
}
Ex: I will likely think about you, \\pauseif I see a rock or a plant or such that has a similar shape to you. \\pauseSometimes, it may even ache a little bit. # time: 8
Ex: But mostly, I will be too occupied with all the discoveries I am making. # time: 8
MC: What's it like on the surface? #speed: slow # time: 4
Ex: It's-- # time: 3
~pause(2)
Ex: Do you really want to hear about that? # time: 3
~pause(2)
MC: I guess not. \\pauseNot really. # time: 5 # speed: slow
-> END

=== intro5 ===
# ambient
~ intensity=5
~ changeDialogueView(2)
~ pauseTutorial(true)
MC: What's that, up ahead? ...It looks frightening. # time: 4
Ex: It's part of all the changes that are happening. Everyone is doing "that" now. # time: 6
MC: But it... it didn't really happen, did it? Between us? # time:5
Ex: No. # time: 3
~ loadInt("showcaseMode")
{ showcaseMode==0:
    MC: And this conversation.. # time: 3
    Ex: There was a song I left. On the coralnet. # time: 3
    ~ loadCutscene=true 
    MC: It was very short. # time: 3
-else:
    ~ loadCutscene=true 
}
~ pause(2)
MC: I wish you... Wait. Are those.. Is that our -- # time: 4
-> END





