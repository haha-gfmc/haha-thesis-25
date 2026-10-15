INCLUDE Functions.ink

VAR npcsTalkedTo=0

=== beginning ===
~pauseTutorial(true)
~npcsTalkedTo=npcsTalkedTo+1
Teacher: Hey. \\pause\\pause\\pauseHow's the current for you? #speed: 50
+   [Fine.]
    MC: I think it's fine.
+   [Too hot.]
    MC: Way too hot.
+   [Too cold.]
    MC: Really cold.
- Teacher: I see. #speed: slow
~ stopSinging()
I don't think I agree. #speed: fast
~ toggleSingingMode()
~ continueSinging()
I'm going to the party next. #speed: normal
~ setDialogueBubble("floral")
This is the floral bubble. It could be used maybe for coral text or something really esoteric and cool.
~ setDialogueBubble("bone")
This is the bone bubble. It could alternativle be used for coral text or something kind of technologic?
Number: {npcsTalkedTo}
~pauseTutorial(false)
{ npcsTalkedTo > 2:
    ~finishTutorialPart(6)
}
-> END

=== ambientDialogue ===
# ambient
Teacher: I like this. # time: 3
~ pause(4)
Teacher: Do you? # time: 3
~ pause(4)
MC: Feels pretty good. #time: 3
-> END

=== teacherBeforeSex ===
~ stopSinging()
Teacher: How did you find the party?
MC: It was alright...
Teacher: What's wrong?
MC: Nothing.
MC: ...
Teacher: Are you still upset that they left you?
MC: No! I don't care about that anymore.
MC: I just want to forget.
~pause(2)
Teacher: Do you wanna race?
MC: Now?
Teacher: When else?
+   [Let's race.]
    MC: Ok let's do it.
    Teacher: We can start slow.
    ~ nextBrain()
+   [No.]
    MC: I'm not in the mood.
    Teacher: Let me know if you change your mind. I might be here still.
- -> END

=== teacherDuringRace1 ===
# ambient
Teacher: Are you good? # time: 2
MC: I'm fine. # time: 2
-> END

=== teacherDuringRace2 ===
# ambient
Teacher: Do you feel it building? # time: 2
MC: Feel what? # time: 2
Teacher: Oh. Sorry. I wasn't thinking... # time: 4
MC: Oh! No, I get it. I think.. I do. # time: 5
-> END

=== teacherDuringRace3 ===
# ambient
Teacher: So have you done it before? # time: 3
MC: No # time: 2
Teacher: Would you like to try it? With me? # time: 6
MC: Y-yeah. Okay. I think it's coming out. #time: 6
Teacher: Let's go somewhere else. #time: 4
~ loadLevel("SexPrototype")
-> END


=== teacherDuringAftercare1 ===
# ambient
Teacher: So what did you think?
MC: What did I think?
Teacher: How was it for you?
~pause(5)
MC: Good? # time:4
MC: Was it good for you?
Teacher: Yeah.
Teacher: I mean, you're a bit inexperienced.
MC: You're not?
Teacher: I've done it a few times.
~pause(5)
MC: I'm sorry if I let you down.
Teacher: Oh no! Not at all.
Teacher: It was sweet.
MC: Thanks.
~pause(5)
Teacher: So...
Teacher: Are you excited for your transformation?
MC: What transformation?
Teacher: You know...
Teacher: When we all get up there.
MC: What?
Teacher: What?
-> END

=== teacherAfterAftercare ===
MC: What did you say?
Teacher: When we all get up there?
MC: What do you mean by that?
Teacher: Well, you know...
Teacher: They're saying that with all these changes to our bodies, we'll soon be able to move to the over-world.
Teacher: I'm sure you've heard that too, right? Some of us have already gone up there.
MC: I... Why would I want that?!
Teacher: What...? I didn't say that you did..
MC: Is this why you wanted to do this with me? To make us change?
Teacher: No! I mean, maybe there is some sort of relationship between the two, but I just wanted to have some fun with you.
MC: I...
MC: I have to go.
-> END







