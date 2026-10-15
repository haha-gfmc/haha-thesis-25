INCLUDE Functions.ink

VAR talkedToVirgin=false


=== npcStart2 ===
~world=2
-> npcStart ->
->->

// * IMPORTANT DIALOGUE *

//You meet them at the smoking area.
// establish: MC feelings after ch1, feelings about world 2
// difference as opposed to prev world 2 parties,
// virgin being disgusted by sex
//    HMM this is probably too long, maybe could be broken up in more convos.. but fear played wont know that u can talk several times
=== virgin ===
# color: ffae1e
-> npcStart2 ->
~ activateBorder("floral",true)
~ activateBorder("rave",false)
NPC: Friend.
{ 
    - talkedToVirgin:
        NPC: Be careful in there.
        // I feel like the virgin should just say ... here because of their disapproval
        //[RESOLVED?] made it a bit more curt but don't wanna just end it
        MC: I will be.
    - else:
        -> chat ->
}
- -> npcEnd ->
~ switchObject("Roadblock 5",false)
~changeDesire("<br>Find <br>Enkidu.")
-> END
=chat 
Care for a chat?
+   [Let's.]
    MC: Sure.
    ~talkedToVirgin=true
+   [Maybe later.]
    MC: Maybe later. There are things on my mind.
    -> npcEnd ->
    -> END
- NPC: How's your current?
+   [Crowded.]
    MC: Feels suffocating.
    I can't swim a pace without bumping into others doing... whatever.
    NPC: I know. At least here it's a bit calmer.
    But you still can't escape that incessant thumping...
    MC: Yeah. I had to get out of there.
+   [Cozy.]
    MC: It feels as if I am enveloped in warmth and festivities.
    It's kind of nice. I don't have to be just me.
    NPC: Really? 
    Well. At least you're not spending all your time in the coralnet anymore.
    MC: ...Sure.
    There isn't any in this world anyway.
    MC: I mean, it's also all kind of overwhelming. That's why I'm here now.
+   [Liberating.]
    MC: It has a fresh, liberating taste. 
    I believe I could act however I want to here, and no one would care.
    NPC: What do you mean by that? Act like how?
    MC: Uhm... I don't know. I was just saying.
    NPC: No, really. What would you do here that you wouldn't do in other instances?
    MC: I don't know! It's just... nevermind.
    // I think MC might not be as honest at their feelings here
    // I feel like a shy person would try to cover it up or try to act normal here because the virgin is definitely someone they feel less comfortable talking about sex and inner feelings with. Its like MC accidently opened up a bit and got disapproved so now they are pretending nothing happened
    //[RESOLVED?]
    NPC: ...
    NPC: I know you think I'm judgemental.//ISABELLE could think through a slightly judgemental/tension point for the crowded option too, because both cozy and liberatng options have a moment of NPC being judgey 
    MC: I wasn't saying that.
    Besides, it's also a lot. I couldn't really bear it anymore so I had to come out here.
    //ISABELLE this is a little repetitive if you choose the Crowded option
- NPC: I see.
I'm sure you've... seen a lot in there.
MC: Yeah. There's a lot going on.
NPC: I used to enjoy these parties. Really. But they've... perverted it. It's against current.
MC: ...It's not that bad.
NPC: So, \\pausehow have you been feeling? I know that last one you had relations with went away as well...
+   [Fine.]
    MC: I'm fine. Really.
    NPC: Really?
    MC: Verily.
    NPC: You know...
    NPC: Just because both of your exes went to the surface...
    MC: I wouldn't call that second one that. Ex. It was just an experiment. 
    NPC ...in a row doesn't necessarily mean that it's a pattern. Lots of people nowadays are heading up there and...
    MC: I said it's fine!!! I don't think it's a pattern!!!
    I don't even need that sort of stuff anyway....!!!
    // I think MC might be a bit more passive agressive here instead of being super up front and honest about their feelings
    //[RESOLVED?]
    NPC: ....
    If you put it that way.
+   [Bad.]
    MC: Bad.
    NPC: Ah.
    ~pause(2)
    NPC: Care to elaborate?
    MC: ...
    NPC: Please?
    MC: Everything is... off. 
    Everyone is doing this thing and it feels like... like they're in on something that I don't necessarily understand and --
    NPC: You know, just because both of your exes went to the surface...
    MC: What? I wasn't thinking about that. And I wouldn't even call that last one an EX.
    NPC: ...in a row doesn't necessarily mean that it's a pattern. Lots of people nowadays are heading up there and...
    MC: I-- I don't care! It's not about them!! #speed: fast
    ~pause(2)
    NPC: Your feeling of loneliness is really worrying to me.
    MC: I think you're making too big a deal out of it.
    NPC: Getting that attached to someone is unnatural. Perhaps you too are changing.
    MC: Aghhhh!!
    NPC: It's okay. Maybe if you just eat leaner fish and incorporate more rock salt in your diet and --
    MC: Can we sing of something different?
    NPC: ....
    Fine.
+   [Not gonna sing about it.]
    MC: I don't want to sing about it.
    NPC: It doesn't necessarily mean to be a pattern, you know. Lots of people nowadays are going to the surface, and just because your two exes...
    MC: I said I don't want to sing about it! Besides, I wouldn't even call that last one "my ex"..! #speed:fast
    NPC: Burying things like this won't do you any good.
    MC: I'm not burying.
    NPC: Well, sure, except that you are.
    MC: I'm taking the space and time to process everything and --
    NPC: A lot of time.
    AND just because no one ever wants to stay with me doesn't mean I'm sad, or -- 
    Or whatever.
    NPC: ....
    If you put it that way.
- NPC: Anyway...
Did you see that Enkidu person? What's their deal?
The name? I've never needed one. Neither has anyone else I know. What makes them think they're so important?
MC: Well it does make it easier to refer to them. This one and that one... It's not very clear.
NPC: I've never had any problems communicating who I'm talking about at any instance.
MC: Sure, okay. But yeah, it's strange.
Still. There's something kind of intriguing about them.
NPC: Intriguing? Really?
MC: ...
NPC: No, really. Say more?
MC: I don't know. I guess I just wonder what it must be like to have that much attention put upon you.
NPC: Must be asphyxiating. //im trying to think of a better word that also would make sense for a fish to say
// haha i think this is fish vocab
MC: I'm sure, yeah.
...
Can I ask you a question?
NPC: Sure.
MC: Why do you hate the entanglement so much?
NPC: Well, hate is a strong word. My emotions are always in balance.
MC: ...Right.
NPC: Regardless. I just don't trust it. We never used to do it, so why now?
MC: Yeah...
And the whole deal with the surface...
NPC: Well, I now think that part is fine with me. 
If people want to try their luck up there, it's none of my business. We're better off with less floaters around here anyway..
MC: What does that even mean?
NPC: Floaters. You know. Anyway.
It's just the act itself... The way those two organs just come out... I wish I didn't know that I could grow one to be frank. I hope I don't.
And the way they writhe and... Ugh! It nauseates me to even think about it.
MC: How do you even know the way they look?
NPC: Are you kidding? Everyone has their organs out in there! It's repugnant.
Anyway...
What now? Do you want to take a stroll around the edge?
MC: Actually...
I was thinking, maybe I'd go back in there for a bit.
NPC: Really?
MC: Yeah, I don't know. I guess I'm just curious to see if anything big happened. Maybe Enkidu dropped their vizor or something. Haha.
NPC: That's not very funny.
MC: ...
NPC: Well, be careful in there please. 
I know you've done "it" before but with all the other changes you never know what might happen this time. You still might get eaten.
MC: People usually ask before it really doesn't happen that often.
MC: And, besides, that's not why I'm going back in there.
NPC: Right.
MC: Right.

    // Same here I think overall its good but MC might be more hiding their feelings? I feel like talking to the virgin is somewhat like how I used to talk to my straight friends in middle school and I was trying very hard to pretending to be nonchalant because I dont want to be seen as a freak? I feel like MC with the virgin has this kind of energy so maybe the expressions of feelings could be more subtle here?
    //[RESOLVED?]
->->


VAR metEnkidu=false

=== EnkiduAtParty1 ===  //Room 3
# color: 2A3B5A
# outline: FFF383
-> npcStart2 ->
~changeDesire("<br>Find a way out of <br>the party.")
Enkidu: Hello there!
MC: ...Hello?
Enkidu: That's how they start conversations on the surface!
MC: Oh...
~ metEnkidu=true
~ changeStartKnot("EnkiduAtParty2")
~ switchObject("Roadblock 1",false)
-> npcEnd ->
-> END
//ISABELLE possibility-MC shares slang they know from the surface but it's wrong or they get embarassed?
// could be fun if this dialogue is initiated by enkidu

//Maybe for this one they're hanging from the ceiling or sth crazy/not dancing
=== EnkiduAtParty2 ===
# color: 2A3B5A
# outline: FFF383
-> npcStart2 ->
Enkidu: You are not dancing!
MC: Neither are you.
Enkidu: I'm using psychic power to recenter the party vibes!
-> npcEnd ->
-> END

=== EnkiduAtParty3 === //Room 5
# color: 2A3B5A
# outline: FFF383
-> npcStart2 ->
~continueSinging()
~ setFMODGlobalParameter("npcSingingVolume",0.8)
Enkidu: You are still not dancing!
MC: I'm aware.
Enkidu: Make that be different!!
//ISABELLE could be a fun flirt moment for Enkidu?
~changeStartKnot("EnkiduAtParty4")
~ switchObject("Roadblock 2",false)
-> npcEnd ->
-> END

=== EnkiduAtParty4 ===
# color: 2A3B5A
# outline: FFF383
-> npcStart2 ->
~continueSinging()
~ setFMODGlobalParameter("npcSingingVolume",0.8)
MC: So... About your whole "name" thing --
Enkidu: Are you seriously trying to have a conversation with me right now??
MC: Yes?
Enkidu: Dance! Now!
~changeStartKnot("EnkiduAtParty3")
-> npcEnd ->
-> END

=== EnkiduAtParty5 ===
# color: 2A3B5A
# outline: FFF383
-> npcStart2 ->
~continueSinging()
~ setFMODGlobalParameter("npcSingingVolume",0.8)
Enkidu: Little minnow!
MC: What?
Enkidu: That's you! That's what you look like!
MC: ...
~changeStartKnot("EnkiduAtParty6")
-> npcEnd ->
-> END

=== EnkiduAtParty6 === // White Room 1
# color: 2A3B5A
# outline: FFF383
-> npcStart2 ->
~continueSinging()
~ setFMODGlobalParameter("npcSingingVolume",0.8)
Enkidu: I like your style!
MC: I think I'm acting really awkwardly...
Enkidu: You are! There's a quiet melancholy about you! It's so different! I love it!
~changeStartKnot("EnkiduAtParty5")
~ switchObject("Roadblock 3",false)
-> npcEnd ->
-> END


=== EnkiduAtParty7 === // Room 7
# color: 2A3B5A
# outline: FFF383
-> npcStart2 ->
~continueSinging()
~ setFMODGlobalParameter("npcSingingVolume",0.8)
Enkidu: Meet me later!
MC: Why? I mean, um. Where? #speed: fast
Enkidu: You'll find me!
~ switchObject("Roadblock 4",false)
-> npcEnd ->
-> END


VAR talkedToEnkiduBeforeMainAct1=false
VAR mcName=""
VAR chosenName=false

=== EnkiduBeforeMainAct1 ===
# color: 2A3B5A
# outline: FFF383
-> npcStart2 ->
{talkedToEnkiduBeforeMainAct1==false:
    ->chat->
}
Enkidu: {Let's do it.|Come on.|There is only one time.|I want to entangle with you.}
// I feel like it starts a bit too abruptly? I feel like as experienced as enkidu they will do some starter just to make MC feel more comfortable like "you know what it means when we are here alone right" or some complements like "I might sound like a freak but I know I like you at the first glance, I just have to get it out"
    //[RESOLVED?]
Now.
+   [Yes!]
    MC: Don't we need to harmonize more first?
    Enkidu: You don't need to when you're with me little minnow.
    Enkidu: Just let it out.
    ~loadLevel("Main Act 2 - 1")
    MC: Oh... Oh!
    // Transition to Main Act
+   [No!]
    MC: No! Uhmm! Not yet! I'm not ready!
    Enkidu: I will not be here forever...
    {But take your time. I love that you're assertive.|But I will be for a bit more.|}
--> npcEnd ->
-> END
= chat
MC: Wow. It's so much quieter in here.
~talkedToEnkiduBeforeMainAct1=true
Enkidu: Do you like it?
+   [Yes.]
+   [No.]
+   [It's fine.]
- MC: It's --
Enkidu: I would like to do it with you.
MC: Oh!
Enkidu: Would you like to as well?
MC: Y-yes.
If that's not too forward.
Enkidu: Sweetheart, I'm all about forwardness.
->->

// Main act happens, then short climax interrupted by Enkidu

VAR feelingsCounter=0

=== EnkiduAftercareTest ===
# color: 2A3B5A
# outline: FFF383
MC: You stopped us there.
Enkidu: I did.
+   [I was disguted.]
    MC: I was... disgusted.
    Enkidu: Why?
    ~feelingsCounter+=1
+   [I was afraid.]
    MC: I was afraid.
    Enkidu: Of?
    MC: I'm not sure. Well... of changes, I guess, happening to me. That I don't understand.
    ~feelingsCounter+=1
+   [I was angry.]
    MC: I was angry.
    Enkidu: At?
    ~feelingsCounter+=1
- Enkidu: Ok I see.
-> EnkiduAftercareTest

// goals: MC talks about ambivalence of sex, enkidu proposes naming
=== EnkiduAftercare1 ===
# color: 2A3B5A
# outline: FFF383
~pause(3)
MC: You stopped us there. 
Enkidu: I did.
MC: Why?
Enkidu: I wanted the moment to last longer.
MC: Ah...
I see.
~pause(4)
MC: To be honest...
Enkidu: Yes?
MC: I didn't think I was going to entangle ever again.
~ finishTutorialPart(0)
Enkidu: And why is that?
->part1

=part1
//Maybe at this point loop and explore all of these
*   [I was disgusted.]
    MC: I was... disgusted.
    Enkidu: Why?
    MC: Well it's... kind of gross, isn't it?
    Enkidu: In what way?
    MC: I don't know. The shape of the organs. And the residue. The writhing. It's... And yet --
    Enkidu: And yet, it feels like nothing else. Doesn't it?
    MC: ...Yeah. I was about to say something like that.
    ~feelingsCounter+=1
*   [I was afraid.]
    MC: I was afraid.
    Enkidu: Of?
    MC: I'm not sure. Well... of changes, I guess, happening to me. That I don't understand.
    // Enkidu: And why is that bad?
    // MC: Well, the changes could be irreversible.
    // Enkidu: Why be afraid of something that feels so wonderful? Change is the most natural thing to happen to anything.
    // MC: ...is it? 
    MC: It felt so good, but also as if... as if I was standing at the precipice of something completely foreign.
    Enkidu: And why is that bad?
    MC: Uhm... the changes could be irreversible.
    Enkidu: And?
    MC: What if I don't like the changes?
    //Enkidu: Yet it feels marvellous, doesn't it?
    Enkidu: What if you love them?
    MC: ...
    I suppose I hadn't considered that.
    // Enkidu: Why be scared of what feels good?
    // MC: ...
    // I don't know.
    ~feelingsCounter+=1
*   [I was angry.]
    MC: I was angry.
    Enkidu: At?
    MC: My last partner. And...
    And the one before that, too.
    Enkidu: Because they left you?
    MC: Uh... I guess, yeah.
    Enkidu: And you're still here.
    MC: Yeah. Wait, how did you know that they left me?
    Enkidu: You have that air about you. Anyway...
    Why blame the entanglement? It sounds like it's them that you're angry at. 
    The act is just what it is. An act.
    MC: Well... When you put it that way..
    ~feelingsCounter+=1
- ->part2

=part2
{
    - feelingsCounter<=2:
        Enkidu: But there's more to it, isn't there?
        -> part1
    - else:
        Enkidu: The way I see it -- if something feels this good, why resist it?
        ~ finishTutorialPart(1)
        -> part3
}

= part3
Enkidu: You say you fear change. Yet, isn't it in our nature to go with the current?
MC: Huh.
I guess so.
Enkidu: You don't sound so convinced.
MC: Well... There's still so much I don't understand.
Enkidu: Such as?
MC: For example...
Well, your name, for example.
Enkidu: What about it?
+   [Why have one?]
    MC: What's the point of having one?
    Enkidu: I suppose... I enjoy having something that is entirely mine, and I enjoy hearing it inside others' minds.
+   [How did you pick it?]
    MC: How did you pick it?
    Enkidu: I liked the way it sounded.
-MC: "Enkidu"... Huh.
Enkidu: Would you like one too?
MC: ...Oh!
Hmm... Would I?
+   [Yes.]
    MC: Yes, I think I'd like to try it.
    // Naming section (the player can name the main character)
    //At this point, transition to overworld?
    ~loadLevel("Naming")
    ~chosenName=true
+   [No.]
    MC: No, I think I'm okay with not having one.
    //At this point, transition to overworld?
    Enkidu: That's perfectly fine. You should have it your way. And no one else's.
    MC: Yeah. I agree.
    ~saveValue("mcName","")
    ~loadLevel("Foreplay 2")
    ~pause(2)
- -> END

=== EnkiduForeplay1 ===
# color: 2A3B5A
# outline: FFF383
-> npcStart2 ->
~stopSinging()
~pause(2)
~loadString("mcName")
{ mcName!="":
    Enkidu: {mcName}. Very... evocative.
    MC: Do you like it?
    Enkidu: What matters is whether YOU like it, little minnow. {mcName}.
    MC: Yeah.
}
MC: Well...
MC: What now?
Enkidu: Now...
I'd like to introduce you to some friends.
// A lot of NPCs enter. 
// Transition to harmonizing/foreplay with everyone
-> npcEnd ->
~ nextBrain()
-> END

//After harmonizing, we enter a cuddle/aftercare scnene with Main Character stroking a big pile of bodies
// It would be cool if we can do a big "collective" speech bubble that is a bunch of overlapping ones at once
=== OrgyAftercare1 ===
~pause(3)
MC: This feels...
NPC2: Ecstatic?
NPC3: Euphoric!
NPC4: Transcendental!!!
MC: Yes...
~ finishTutorialPart(0)
~ finishTutorialPart(1)
And also...
+   [Snug. #place: 1]
    NPC2: Snug?
+   [Warm. #place: 3]
    NPC2: Warm?
+   [Electrifying. #place: 6]
    NPC2: Electrifying?
+   [Serene. #place: 7]
    NPC2: Serene?
+   [Thrilling. #place: 5]
    NPC2: Thrilling?
- MC: Yeah! Exactly...
~ finishTutorialPart(2)
NPC5: I've never felt this connected before...
NPC3: There's nothing like it.
MC: Before this, when I was alone...
It was..
+   [Cold. #place: 0]
+   [Suffocating. #place: 2]
+   [Burning. #place: 4]
+   [Throbbing. #place: 6]
- NPC4: It was as if there was no one else in the world.
MC: It seemed that no matter how hard I tried...
NPC5: To reach out to others.
NPC2: To occupy myself.
NPC3: To stop thinking about it.
NPC1: It felt like it would always be that way.
MC: It felt like no one was ever going to..
+   [Understand me. #place: 3]
    MC:...understand me again.
+   [Hold me. #place: 7]
    MC:...hold me again.
+   [Love me. #place: 9]
    MC:...love me again.
- NPC5: But now, everyone's thoughts...
NPC1:...feelings...
Enkidu: Desires.
MC:...are like my own.
NPC4: There's no need to be..
+   [Afraid. #place: 4]
+   [Bitter. #place: 5]
+   [Hopeless. #place: 6]
- MC: ..again.
~pause(4)
NPC4: I want to be closer.
NPC5: Me too. # stayonscreen # speed:20 # ambient # time:1
NPC1: Me too.  # speed:fast # stayonscreen # time:0.2
NPC2: Me too.  # speed:fast # stayonscreen # time:0.2
NPC3: Me too.  # speed:fast # stayonscreen # time:0.2
NPC4: Me too.  # speed:fast # stayonscreen # time:0.2
~loadLevel("")
NPC11: Me too.  # speed:fast # stayonscreen # time:0.2 # notambient
//These would appear everywhere and be hard to avoid
+   [Me too. #place: 0]
+   [Me too. #place: 1]
+   [Me too. #place: 2]
+   [Me too. #place: 3]
+   [Me too. #place: 4]
+   [Me too. #place: 5]
+   [Me too. #place: 6]
+   [Me too. #place: 7]
+   [Me too. #place: 8]
- MCSmall: Me too.
-> END

//Orgy main act + climax happens
//Go back to aftercare orgy/cuddle

=== OrgyAftercareTest ===
MC: I..
I want to...
+   [Go. #place: 1] 
    NPC1: Go.
+   [Change.#place: 3] 
    NPC2: Change.
+   [Move. #place: 6] 
    NPC3: Move.
+   [Grow. #place: 7] 
    NPC4: Grow.
- MC: Yes...
Maybe sometime soon...
NPC6: I can't wait. # speed:fast # stayonscreen # ambient # time:0.2
NPC7: I want it now.. # speed:fast # stayonscreen # time:0.2
NPC8: I'm counting the cycles... # speed:150 # stayonscreen # time:0.2
NPC9: To be on the surface. # speed:150 # stayonscreen # time:0.2
NPC10: To be UP THERE. # speed:120 # stayonscreen # time:0.2
MCSmall: I-- # notambient
-> END

=== OrgyAftercare2 ===
~pause(2)
MCSmall: I...
NPC1: I wish... # stayonscreen # speed:20 # ambient # time:1
// pop these lines one after the other while leaving previous ones on screen
NPC4: If I could... # stayonscreen # speed:20 # time:1
NPC5: I'd like to... # stayonscreen # speed:20 # time:1
MCSmall: If this could last forever... # stayonscreen # notambient # speed:20 # time:1
~clearScreen()
~pause(2)
NPC6: I can't wait. # speed:fast # stayonscreen # ambient # time:0.2
NPC7: I want it now.. # speed:fast # stayonscreen # time:0.2
NPC8: I'm counting the cycles... # speed:150 # stayonscreen # time:0.2
NPC9: To be on the surface. # speed:150 # stayonscreen # time:0.2
NPC10: To be UP THERE. # speed:120 # stayonscreen # time:0.2
MCSmall: I-- # notambient # stayonscreen
~clearScreen()
~pause(2)
MC: Is this what life is like? On the surface?
NPC4: I hear they do this everyday. 
NPC1: Or maybe every other day.
NPC2: I hear gravity makes it even better!
Enkidu: What is everyone the most excited to do on the surface?
NPC1: I want to count stars! # ambient # speed:20 # stayonscreen # time:1.2
NPC2: I want to cross a street! # speed:20 # stayonscreen # time:1.3
NPC4: I want to eat a tree! # speed:20 # stayonscreen # time:1.2
NPC5: I want to crochet! # speed:20 # stayonscreen # time:1.2
NPC3: I want to walk on clouds! # speed:20 # stayonscreen # time:1.3
NPC11: I want to lick a fire! #notambient # speed:20 # stayonscreen # time:1.2
~clearScreen()
Enkidu: And what about you?
MCSmall: I...
I want to...
+   [I want to see them again. #place: 0]
- MC: Sorry, I...
NPC2: It's okay.
NPC1: It's okay to not be whole.
NPC5: But we have each other.
NPC7: We're together now. # speed:fast # stayonscreen # ambient # time:0.3
NPC8: We're together now. # speed:fast # stayonscreen # time:0.3
NPC9: We're together now. # notambient # speed:normal # stayonscreen # time:0.3
~clearScreen()
Enkidu: Yes. You don't have to worry about what comes next.
NPC6: Focus on the now. # speed:fast # stayonscreen # ambient # time:0.3
NPC9: Focus on the now. # speed:fast # stayonscreen # time:0.3
NPC10: Focus on the now. # notambient # speed:normal # stayonscreen # time:0.3
+   [Now... #place: 4]
+   [Now. #place: 5]
+   [Now! #place: 7]
- ~clearScreen()
MCSmall: Maybe...
Maybe it will be okay...
Up there...
I want to...
+   [Go. #place: 1]
    NPC6: Go. # speed:fast # stayonscreen # ambient # time:0.1
    NPC7: Go. # speed:fast # stayonscreen # time:0.1
    NPC8: Go. # speed:fast # stayonscreen # time:0.1
    NPC9: Go. # speed:fast # stayonscreen # time:0.1
    NPC10: Go. # speed:slow # stayonscreen # time:0.1 # notambient
+   [Change. #place: 3]
    NPC6: Change. # speed:fast # stayonscreen # ambient # time:0.1
    NPC7: Change. # speed:fast # stayonscreen # time:0.1
    NPC8: Change. # speed:fast # stayonscreen # time:0.1
    NPC9: Change. # speed:fast # stayonscreen # time:0.1
    NPC10: Change. # speed:slow # stayonscreen # time:0.1 # notambient
+   [Move. #place: 6]
    NPC6: Move. # speed:fast # stayonscreen # ambient # time:0.1
    NPC7: Move. # speed:fast # stayonscreen # time:0.1
    NPC8: Move. # speed:fast # stayonscreen # time:0.1
    NPC9: Move. # speed:fast # stayonscreen # time:0.1
    NPC10: Move. # speed:slow # stayonscreen # time:0.1 # notambient
+   [Grow. #place: 8]
    NPC6: Grow. # speed:fast # stayonscreen # ambient # time:0.1
    NPC7: Grow. # speed:fast # stayonscreen # time:0.1
    NPC8: Grow. # speed:fast # stayonscreen # time:0.1
    NPC9: Grow. # speed:fast # stayonscreen # time:0.1
    NPC10: Grow. # speed:slow # stayonscreen # time:0.1 # notambient
    // typo here?
+   [Transform. #place: 9]
    NPC6: Transform. # speed:fast # stayonscreen # ambient # time:0.1
    NPC7: Transform. # speed:fast # stayonscreen # time:0.1
    NPC8: Transform. # speed:fast # stayonscreen # time:0.1
    NPC9: Transform. # speed:fast # stayonscreen # time:0.1
    NPC10: Transform. # speed:slow stayonscreen # time:0.1 # notambient
- ~clearScreen()
MCSmall: Yes...
// At this point begin to fade to black
Maybe sometime soon...
NPC6: We'll. # speed:20 # stayonscreen # ambient # time:0.4
NPC7: All. # speed:20 # stayonscreen # time:0.4
NPC8: Be. # speed:20 # stayonscreen # time:0.4
NPC9: Together. # speed:20 # stayonscreen # time:0.6
NPC10: We'll all be together. # speed:30 # stayonscreen # time:0.4 # notambient
~clearScreen()
~pause(2)
//MC's hand can become slack or withdraw slowly/they're falling asleep
// it continues fading to black, and overshadows the dialogue too/u can't see it to completion
NPC1: I really can't wait.
NPC4: Can we go now?
NPC2: Through the cone! To the surface!
NPC5: I want someone to hold my hand when we go.
NPC2: I want to be in front!
NPC3: We should go now.
~ loadLevel("")
NPC4: Now! Now!
NPC1: There's only now.
NPC5: Let's go now!
NPC2: Now! Now! Now!
// maybe mention the tunnel here? 
->END

//MC wakes up later. everyone is gone. they go to the center near the big tunnel to the surface and they find the janitor.
VAR talkedToJanitor=0
=== janitor ===
-> npcStart2 ->
{ 
- talkedToJanitor==0:
    MC: Where is everyone?
    Janitor: Where do you think?
    MC: They-- But--
    ...
    They went to the surface?
    Janitor: Wasn't it what this was all about?
    MC: But they.. went without me.
    Janitor: Weren't you snoozing back there?
    MC: ...
    Janitor: Hey... they probably just didn't want to bother you.. I mean, the way is still open if you wanna join them. Just up that cone.
    MC: I...
    I don't know.. if I'm ready.
    Janitor: Well there's only one way to find out, isn't there?
    MC: But I'm -- what if --
    ...
    Janitor: Sounds like you've got a choice to make.
    MC: ...
    Janitor: Well, you ponder that. I've got to get back to it.
    But.. feel free to sing to me more, if you need a sounding board. I'll be here.
    ~talkedToJanitor=talkedToJanitor+1
- talkedToJanitor==1:
    MC: What are you still doing here?
    Janitor: Oh, I like to stick around here for a bit longer and clean up after everyone's partied out. Someone's got to do it.
    MC: ...I see.
    MC: And did everyone really..?
    Janitor: Well, not everyone. Those more old-fashioned like myself are starting to migrate to the next world. I'll join them once I feel like I've done enough.
    But something tells me there won't be a party like this one again, so... maybe I can just wing it.
    ~talkedToJanitor=talkedToJanitor+1
- talkedToJanitor==2:
    MC: Is it really time for migration? I have the impression it usually takes longer than this.
    Janitor: Well, when you feel the call. You've gotta heed it, right?
    MC: I guess.
    Janitor: All the others moving up also maybe sorta... steered the current a different way, maybe. 
    Kind of a mood killer I suppose.
    MC: ...
    ~talkedToJanitor=talkedToJanitor+1
- talkedToJanitor==3:
    Janitor: {So what do you think you're gonna do?|You change your mind?}
    +   [I'm going up.]
        MC: I'm going to follow them up.
        Janitor: You sure?
        MC: ...
        No.
        -> npcEnd ->
        -> END
    +   [I'll stay & migrate.]
        MC: I think I'll stay.
        Janitor: You sure?
        MC: ...
        No.
        -> npcEnd ->
        -> END
    +   [I don't know.]
        MC: I don't know. I'm...
        Janitor: Scared, huh?
        MC: ...
        Janitor: But you don't wanna get left behind.
        MC: It's...
        ...
        -> npcEnd ->
        -> END
}
-> npcEnd ->
-> END

// At this point, what if you can actually choose to try the surface or just migrate? :thinking:
// I guess there'd be less emotional impact if you do the former


// When you go to surface, MC can't breathe

// Then migration





// =*=*=*=* OVERWORLD NPCS *=*=*=*=










=== alienDancer1 ===
# color: d6eaf6
-> npcStart2 ->
NPC: Let's dance! Dance! Dance!!!
-> npcEnd ->
-> END


=== alienThisIsWhatTheyDo === 
# color: d6eaf6
-> npcStart2 ->
NPC: This is what they do on the surface!!!
-> npcEnd ->
-> END

=== alienSuit1 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: Do you like my suit??
MC: What's it made of?
NPC: If you're wondering whether it's edible, it's not!!!
-> npcEnd ->
-> END

=== alienSuit2 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: Ever since Enkidu started wearing clothing, we knew we had to do the same!
And now look at us! So beautiful!
-> npcEnd ->
-> END

=== alienSuit3 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: I wear this to show my belonging to my in-group!!!
-> npcEnd ->
-> END

=== alienSuit4 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: On the surface, they have created a highly arbitrary moral system that govern all aspects of their society, so they wear these to hide their true selves!!!
-> npcEnd ->
-> END

=== alienChair1 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: People on the surface have butts that are so bony they need to cushion these! For comfort!!!
I've been starving myself so that you could feel my bones too! Do you want to feel it?!
+   [Yes.]
    NPC: How does it feel???
    MC: Kind of soft.
    NPC: Aghhh!!! It was all for nothing!!!!
+   [No.]
    NPC: Feel free tome back whenever!!!
- -> npcEnd ->
-> END

=== alienChair2 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: On the surface, the pull of the Earth is so attractive that they need to be on these to keep from being swallowed into its inside!!! 
-> npcEnd ->
-> END

=== alienChair3 === //INGAME
# color: d6eaf6
-> npcStart2 ->
NPC: Even though my head is physically beneath yours, me "sitting" is a signifier of my potentially justified feelings of superiority over you!
Verticality has very strong connotations on the surface!!!
-> npcEnd ->
-> END

=== alienWalking1 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: This may be how they physically move their bodies on the surface, but their actual usual means of locomotion are way more complex!!!
NPC: Like, so complex that it tears up every material thing around them! It's spectacular!!!
-> npcEnd ->
-> END

=== alienWalking2 === // ingame
# color: d6eaf6
-> npcStart2 ->
NPC: In the surface, they move around like this because they're born with feet stuck to the ground!
I wonder if it hurts all the time!! Can you imagine the sexual implications?!?!
-> npcEnd ->
-> END

=== alienWalking3 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: On the surface there is this heavy and oppressive substance everywhere that forces them to move like this!!!
-> npcEnd ->
-> END

=== alienJournal1 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: Surfaceans love their own thoughts so much that they've invented this method to materialize it!
The method is so fast that in the blink of an eye they can make copies for everyone in the world to ingest!!!
-> npcEnd ->
-> END

=== alienJournal2 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: Surface people wipe their asses with this!!!
-> npcEnd ->
-> END

=== alienJournal3 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: Have you read the noose? Have you read the noose?
This is how they talk on the surface! I don't know what it means but I love nooses!!!
-> npcEnd ->
-> END

=== alienJournal4 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: I have marked on this shaped rock standardized symbols of my inner being!!! 
-> npcEnd ->
-> END

=== alienStab1 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: This person has wronged me so I take it out on them, but without using my teeth or claws!
It's so sophisticated!!
-> npcEnd ->
-> END

=== alienStab2 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: On the surface they have way more complex means to deal with disagreements, but none of them actually address the problem! 
It's so sophisticated!!!
-> npcEnd ->
-> END

=== alienStab3 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: The most important part of stabbing is to make sure your partner is comfortable and having fun!!!
-> npcEnd ->
-> END

=== alienStab4 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: In the surface, they actually take death very seriously.
So something like this would be considered really bad!!!
-> npcEnd ->
-> END

=== alienAd1 ===
# color: f6e2d6
-> npcStart2 ->
NPC: I made this ad to let people know that they can entangle with me!!! Are you interested? I'm pretty proud of it.
-> npcEnd ->
-> END

=== alienAd2 ===
# color: f6e2d6
-> npcStart2 ->
NPC: On the surface, they use signifiers such as this one to let people know about things that they exchange for "currency".
The nice thing here is that you can entangle with me without a need for goods or services. 
Isn't that enticing?!
-> npcEnd ->
-> END

=== alienAd3 ===
# color: f6e2d6
-> npcStart2 ->
NPC: If you want, you can entangle with me and then give me an object that signifies a form of standardized value!
Isn't that enticing?!
-> npcEnd ->
-> END

=== alienSexTalk1 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: My favorite part about the entanglement? Figuring out what to do with the residue at the end!!!
MC: I didn't ask..?
NPC: This is just how they start conversations on the surface!!!
-> npcEnd ->
-> END

=== alienSexTalk2 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: The entanglement isn't enough for me anymore! I want something deeper!! Something closer!!!
I want to merge everyone's consciousness together until there are no borders between selves!!!
MC: Is such a thing even possible..?
NPC: I hope so!! I can't take being apart anymore!!!!!
-> npcEnd ->
-> END

=== alienSexTalk3 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: They are sucking on each others' organs but without the intention of consuming! I didn't even know you could use mouths for things other than eating!
Can you imagine how many more uses for mouths they have on the surface? Probably more than a million!!!
-> npcEnd ->
-> END

=== alienTomb1 === 
# color: d6eaf6
-> npcStart2 ->
NPC: On the surface, they put the bodies of dead people inside the ground!
I wonder what it feels like.. I can't wait to have it happen to me!
-> npcEnd ->
-> END

=== alienAir1 === 
# color: d6eaf6
-> npcStart2 ->
NPC: Have you tried inhaling "air" yet?
It gives me a rush every puff I take! In the surface they must be euphoric all the time!
-> npcEnd ->
-> END

=== alienAir2 === 
# color: d6eaf6
-> npcStart2 ->
NPC: I keep taking air in until it doesn't hurt all of my insides anymore!
Then I'll know I'm ready to move to the surface!!
-> npcEnd ->
-> END

=== alienArtist1 === 
# color: d6eaf6
-> npcStart2 ->
NPC: On the surface, they have distinctions between those who make art and those who do not, which has a lot of societal and hierarchical implications!
It's so sophisticated! I love it!!
-> npcEnd ->
-> END

=== alienSexTalk4 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: Somehow it arouses me even more to watch people entangling that doing it myself!!!
How does it feel for you?
+   [Painful.]
    NPC: What? That is so weird!!!
+   [Arousing.]
    MC: I think I... well... maybe...
    I don't think the feelings it raises in me are worth listening to.
    NPC: What? That is so weird!!!
+   [Disgusting.]
    NPC: But in a good way???
    MC: I don't think so...?
- -> npcEnd ->
-> END

=== alienSexTalk5 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: My favorite part about the entanglement is that I can feel close to anyone, even oafish idiotic and pathetic people who I normally hate singing to!!!
- -> npcEnd ->
-> END

=== alienSexTalk6 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: I can't wait for the entanglement to replace all forms of socialization!
MC: Is that really going to happen..?
NPC: I hope so! It's so much better than singing and telepathy! 
NPC: Misunderstanding each other is so easy with other means of communications, but the entanglement is so straightforward that there are no chances of ever hurting each other!!!
- -> npcEnd ->
-> END

=== alienSexTalk7 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: You!
NPC: Do you want to do it with me now?!
MC: Sorry, I...
+   [Don't do it.]
    MC: I don't really do it anymore.
    NPC: Why???
    MC: I just... I don't think it's for me.
    NPC: Okay!!! Thank you for clarifying!!
+   [Don't like you.]
    MC: I don't think you're my type. Sorry.
    NPC: That hurts a little bit but I'm sure I can find somebody else to lessen the pain!!
+   [Am not in the mood.]
    MC: I'm uh... not in the mood currently.
    NPC: Do you think you will be soon?!
    MC: Uuuh... I don't know.
    NPC: I suspect you're only saying this to hide your general feelings about me or the entanglement!!
    MC: Ah.
- -> npcEnd ->
-> END

=== alienTalk8 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: Isn't it awesome and so exciting that we keep finding new ways of entangling every day?!
NPC: Unfortunately I've always struggled to live in the moment and I am overcome with fear of the day we've ran out of interesting new ways and grow bored of this as of everything else!!!
- -> npcEnd ->
-> END

=== alienSexTalk9 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: I actually don't enjoy the entanglement physically all that much, but I love learning about all the new things our bodies can do!!!
I'm so excited to see what else may grow when we're on the surface! I hope I get "ears"!!!
- -> npcEnd ->
-> END

VAR npcRubbed=false

=== alienSexTalk10 === //ingame
# color: d6eaf6
-> npcStart2 ->
{ npcRubbed==false:
    NPC: I've recently discovered that I feel ecstatic when someone rubs the tip of my dorsal fin!
    Will you do it for me?
    +   [Rub it.]
        Ahhhh! Thank you! I'm done now! Please leave!!
        ~npcRubbed=true
        -> npcEnd ->
        -> END
    +   [Don't.]
        NPC: Aww okay! It's too bad I think it would be weird if I did it myself!!
        -> npcEnd ->
        -> END
- else:
    NPC: Please leave me alone now!
    -> npcEnd ->
    -> END
}

=== alienSexTalk11 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: I actually have really conflicting feelings about the entanglement that I find difficult to resolve!
MC: Really? Me too! Can you say more?
NPC: Oh, sorry! I was just imitating people on the surface! Apparently they always come up with reasons to feel weird about it!! It's so charming!!!
MC: Oh.
NPC: But let me be clear I LOVE ENTANGLING!!!
- -> npcEnd ->
-> END

=== alienSexTalk12 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: WHEN YOU ENTANGLE DO YOU HAVE VISIONS OF THE END OF THE WORLD???
MC: I don't really --
NPC: SPIKY HOST-LESS SHELLS WILL RAIN FROM THE SKY AND PARENTS WILL TURN ON CHILDREN AND IT IS FORTHCOMING!
- -> npcEnd ->
-> END

=== alienSexTalk13 === //ingame
# color: 1F7A6E
-> npcStart2 ->
NPC: My favorite thing about the entanglement is that I can just do it with anyone at any time and there is no room for jealousy, bitterness, or any other negative feelings!
- -> npcEnd ->
-> END

=== alienSexTalk14 === //in game
# color: d6eaf6
-> npcStart2 ->
NPC: IT'S A LITTLE UNUSUAL THAT WE JUST STARTED DOING THIS WITH NO SEEMING REASON EVEN THOUGH NONE OF OUR ANCESTORS DID IT
MC: Yes! And what's more --
NPC: UNUSUAL IN A FUN AND EXCITING WAY! IT'S SO SPECIAL!!!
- -> npcEnd ->
-> END

=== alienTip1 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: My favorite thing about singing is that I can tone-shift using the FACE BUTTONS!!!
What I mean by that is the NORTH, EAST, SOUTH and WEST BUTTONS!!!
- -> npcEnd ->
-> END


=== alienEnkidu1 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: Have you met Enkidu yet??
+   [Yes.]
    {metEnkidu==false:
        NPC: Are you sure you're telling the truth??
        MC: Sorry.. I'm not sure why I said that.
    - else:
        NPC: They're so hot it makes me want to rip off my skin!!!
    }
+   [No.]
    NPC: You should talk to them!! They may seem very intimidating and charismatic but they're actually super approachable!!!
+   [What?]
    MC: What is Enkidu?
    NPC: Oh! It's a person!
    It's that super hot one with the beautiful headpiece over there!
    They have a "name", a made-up term used to refer to them!!!
    And they are used to speak something true about your person!
    I'm currently workshopping mine! Do you want to know it?!
    ++   [Yes.]
        NPC: ONE-WHO-ENJOYS-EATING-A-LOT-AS-WELL-AS-ENTANGLING!!
        MC: That's nice.
    ++   [No.]
        NPC: You're right I should work on it more before sharing it with just anybody!!!
- -> npcEnd ->
-> END

=== alienEnkidu2 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: ENKIDU IS SO GORGEOUS AND KIND AND BEAUTIFUL AND PERFECT
-> npcEnd ->
-> END

=== alienEnkidu3 === //ingame
# color: d6eaf6
-> npcStart2 ->
NPC: ENKIDU REALLY KNOWS HOW TO THROW A PARTY UNLIKE MOST PEOPLE FOR EXAMPLE YOU AND ME
WE WOULD NEVER KNOW HOW
-> npcEnd ->
-> END



=== shrimptux1 === //ingame
# color: 1F7A6E
-> npcStart2 ->
NPC: This may prove controversial but I am personally not too warm on this "Enkidu" personnage.
They have this tendency of entangling with someone then out-of-the-blue deciding to withhold further entwining because they were not "fun" or "interesting" enough.
(this of course according to their own arbitrary definitions of "fun" and "interesting"-ness...)
To the point where they would not even give me the dignity of harmonizing back when I try to sing with them!!
Ahem.. I misspoke. What I meant when I said "me" was "a friend of mine". You understand.
-> npcEnd ->
-> END 

=== shrimptux2 === //ingame
# color: 1F7A6E
-> npcStart2 ->
NPC: Frankly, there is something "trite" about these parties that was not there before.
I remember, before we began to entangle, we would find much more interesting and novel ways to entertain ourselves..
Now it's all about the entanglement! So cheap and overdone. Bah!
-> npcEnd ->
-> END 

=== shrimptux3 === //ingame
# color: 1F7A6E
-> npcStart2 ->
NPC: What is so disappointing about this cycle's party is that everyone is so busy dancing and entwining, no one has taken the time to make the buffet!
What am I supposed to feast on in-between intense linking sessions? Mere unseasoned live fish? Bah!
It makes me so disgusted, I haven't even been in the mood to entangle with anyone here! Despite the abundance of elfin, magnetic, & willing creatures wherever meets the eye!
-> npcEnd ->
-> END 

=== horma1 === //ingame
# color: 7E0D13
-> npcStart2 ->
NPC: EVERYONE HERE IS SO GORGEOUS, IT'S TOO BAD I'M CURRENTLY ON A "KNOT-LESS" DIET.
+   [What's that?]
    IT'S THIS NEW THING WHERE YOU QUIT ENTANGLING FOR A CERTAIN LENGTHY DURATION.
    APPARENTLY THE LONGER YOU'RE ON IT THE MORE RHAPSODIC THE NEXT ONE WILL BE, SO I'M REALLY EXCITED BUT ALSO I MUST RESTRAIN MYSELF.
    MC: So how long has it been for you?
    NPC: APPROXIMATELY FIVE INSTANTS BUT I'M HOLDING OUT FOR AT LEAST A COUPLE MORE.
+   [Ok.]
    YOU SHOULD TRY IT.
- -> npcEnd ->
-> END 

=== horma2 === //ingame
# color: 7E0D13
-> npcStart2 ->
NPC: DANCING FEELS SO GOOD I WONDER WHAT IT FEELS LIKE ON THE SURFACE.
PROBABLY ABOUT THE SAME BUT WITH MORE TONGUE ACTION?
- -> npcEnd ->
-> END 

=== popero1 === //inside
# color: 966382
-> npcStart2 ->
NPC: I could never get into music and dancing. It feels too aspirational.
- -> npcEnd ->
-> END 

=== eelor1 === //ingame
# color: 6D6787
-> npcStart2 ->
NPC: I've enjoyed myself at this party, but something odd happens where if I spend a lot of time in there I need to spend some time revitalizing away from the crowd.
Is this what the change is like? Am I about to start being pulled to the surface?? I'm not ready!
- -> npcEnd ->
-> END 

=== xrys1 === //ingame
# color: 2b6136
-> npcStart2 ->
NPC: To be honest, I don't really connect with the music here so I'm just gonna wait here until it's time to move on to the next world.
It's too "badoom-tshh POONG POONG" and I'm more into "zaa... PARATATTA shoooo ERN??". You get it?
- -> npcEnd ->
-> END 





