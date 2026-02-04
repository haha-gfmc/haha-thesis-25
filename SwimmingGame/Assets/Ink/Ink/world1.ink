INCLUDE Functions.ink

VAR sexIntensity=0
VAR npcsTalkedTo=0
VAR coralTalkedTo=0
VAR coralToTalkToBeforeProgress=4
VAR followingTeacher=false
VAR talkedToTeacherAtDiner=false
VAR retractHandTrigger=false
VAR libraryOpen=false
VAR hadChatWithFriend=false
VAR desireStep=0
VAR showcaseMode=0
VAR talkedToThisCoralnet=false


=== npcStart1 ===
~world=1
-> npcStart ->
->->

/* CORALNET */

//I'm calling OP initator for now but could be different
// other ideas: first author, singer

=== coralnetStart ===
{ talkedToThisCoralnet==false:
    ~ coralTalkedTo=coralTalkedTo+1
}
~ loadInt("showcaseMode")
{ showcaseMode==1:
    ~coralToTalkToBeforeProgress=2
}
{ coralTalkedTo==coralToTalkToBeforeProgress && talkedToThisCoralnet==false:
    -> coralnetProgress
}
~ setDialogueBubble("bone")
~ npcsTalkedTo=npcsTalkedTo+1
~ stopSinging()
~ pauseTutorial(true)
~ muffleNPCsVolume()
~ muffleSwimmingVolume()

{
    - desireStep==0:
        ~desireStep=1
        ~changeDesire("<br>Read more coralnet.")
    - desireStep==1:
        ~changeDesire("<br>Read more coralnet.")
}
~ activateBorder("coral",true)
->->

=== coralnetEnd ===
~pauseTutorial(false)
{ npcsTalkedTo > 2:
    ~finishTutorialPart(11)
}
~ continueSinging()
~ restoreNPCsVolume()
~ restoreSwimmingVolume()
~ activateBorder("coral",false)
-> END

=== coralnetAddedAnswer ===
~ triggerMetamorphosis()
~ playOneShot("event:/Overworld/Things/Coralnet Metamorphosis",1)
->->

VAR cnetAnswer1=0
=== coralnet1 ===
{coralnet1>1: 
    ~talkedToThisCoralnet=true
- else:
    ~talkedToThisCoralnet=false
}
-> coralnetStart ->
Coralnet: motif: favorite swimming style?
> what's everyone's favorite swimming style and why?
> i like to do hermit on my lower body and cerulean top and i switch to octopus when my arms get tired i find it the most efficient.
> i just kind of do what everyone else is doing
> i love to slide on walls. does anyone else do this
> i like going backwards because it's always a lovely surprise bumping into something
> i like sliding on walls as well
> i alternate
{   
- cnetAnswer1==0:
    Add answer?
    -> answers ->
- cnetAnswer1==1:
    > i enjoy floating with the current and trying to be as still as possible, unnoticeable to everyone around me.
- cnetAnswer1==2:
    > i enjoy being on my back and feeling held by the water.
- cnetAnswer1==3:
    > i like coasting slowly with my arms forward and only moving my legs it's when i do my best thinking.
}
-> coralnetEnd

= answers
+   [Yes.]
    What will you say?
    ++  [I like staying still.]
        -> coralnetAddedAnswer ->
        > i enjoy floating with the current and trying to be as still as possible, unnoticeable to everyone around me.
        ~cnetAnswer1=1
        -> coralnetEnd
    ++  [I like being on my back.]
        -> coralnetAddedAnswer ->
        > i enjoy being on my back and feeling held by the water.
        ~cnetAnswer1=2
        -> coralnetEnd
    ++  [I like coasting slowly.]
        -> coralnetAddedAnswer ->
        > i like coasting slowly with my arms forward and only moving my legs it's when i do my best thinking.
        ~cnetAnswer1=3
        -> coralnetEnd
+   [No.]
->->


VAR cnetAnswer2=0
=== coralnet2 ===
{coralnet2>1: 
    ~talkedToThisCoralnet=true
- else:
    ~talkedToThisCoralnet=false
}
-> coralnetStart ->
Coralnet: motif: has anyone actually done "it" yet?
> motif.
> yes.
> yeah i have.
> i did it with a bunch of people.
> how does it work exactly?
> one of you shrinks down, and you enter the other person's mouth.
> then you tickle the under of their tongue.
> ridiculous. it's really simple, you just have to lie very still next to each other and then it feels good in your abdomen.
> hi! i'm an actual person who's done it, here.
> basically there's two types of people: one with an orifice and one with a long hard object. the long object goes into the orifice many times.
> that's ridiculous. what would even determine if you're a orifice or long object person? plus it sounds highly unpleasant.
> all of these sound disgusting. are people really doing this?
> how much kicking are you supposed to do? does anyone know?
{   
- cnetAnswer2==0:
    Add answer?
    -> answers ->
- cnetAnswer2==1:
    > i am also curious about it and am eager to hear more.
- cnetAnswer2==2:
    > i did it and it was incredible and really undescribable apologies no more details.
- cnetAnswer2==3:
    > a lot of kicking be careful about their teeth. this is the truth because i know because i've done it.
}
-> coralnetEnd

= answers
+   [Yes.]
    What will you say?
    ++  [I want to hear about it.]
        -> coralnetAddedAnswer ->
        > i am also curious about it and am eager to hear more.
        ~cnetAnswer2=1
        -> coralnetEnd
    ++  [I've done it,]
        -> coralnetAddedAnswer ->
        > i did it and it was incredible and really apologies sorry no more details.
        ~cnetAnswer2=2
        -> coralnetEnd
    ++  [Lots of chewing.]
        -> coralnetAddedAnswer ->
        > a lot of kicking be careful about their teeth. this is the truth because i know because i've done it.
        ~cnetAnswer2=3
        -> coralnetEnd
+   [No.]
->->

// Love how they are all straight-up misinformation and
// the reference to human sex
VAR cnetAnswer3=0
=== coralnet3 ===
{coralnet3>1: 
    ~talkedToThisCoralnet=true
- else:
    ~talkedToThisCoralnet=false
}
-> coralnetStart ->
Coralnet: motif: hole in my head
> i woke up with a hole in my head. what do i do?
> please add specificity
> it's a hole on the side of my head. i don't know why it's there.
> initiator are you sure you weren't stabbed?
> have you tried putting things in it?
> yes, it feels a little odd.
> and i always feel a sort of unpleasant pressure so i started plugging it
> why?
> it felt right
> how big is it initiator?
> about the size of my finger.
> initiator i have a lot of experience with orifices could we meet and i could examine you?
> no, it's really weird and i would rather not or else i wouldn't be on here
> initiator any update? i think i have two of them, right above my mouth
> i conceal it with my tongue
> yes it's still really strange but i feel i am becoming stronger and more deadly
{   
- cnetAnswer3==0:
    Add answer?
    -> answers ->
- cnetAnswer3==1:
    > i would continue putting things in it. eventually you might be complete
- cnetAnswer3==2:
    > the hole points to your emptyheadedness. try to absorb more culture and thought.
- cnetAnswer3==3:
    > this makes me very apprehensive initiator. i suggest you banish yourself so as not to infect more of us.
}
-> coralnetEnd

= answers
+   [Yes.]
    What will you say?
    ++  [Plug it.]
        -> coralnetAddedAnswer ->
        > i would continue putting things in it. eventually you might be complete
        ~cnetAnswer3=1
        -> coralnetEnd
    ++  [It's a sign.]
        -> coralnetAddedAnswer ->
        > the hole points to your emptyheadedness. try to absorb more culture and thought.
        ~cnetAnswer3=2
        -> coralnetEnd
    ++  [It's scary.]
        -> coralnetAddedAnswer ->
        > this makes me very apprehensive initiator. i suggest you banish yourself so as not to infect more of us.
        ~cnetAnswer3=3
        -> coralnetEnd
+   [No.]
->->

VAR cnetAnswer4=0
=== coralnet4 ===
{coralnet4>1: 
    ~talkedToThisCoralnet=true
- else:
    ~talkedToThisCoralnet=false
}
-> coralnetStart ->
Coralnet: motif: ate my lover
> is it abnormal to eat your lover without asking if you could?
> they were annoying me a lot and i didn't want to hear their thoughts anymore.
> my swimmates are calling it objectionable behavior.
> initiator why didn't you just stop singing with them?
> it felt rude
> i think it's ok i've eaten 20 of my past lovers and each time they were plumper & tastier
> i think you shouldn't connect with someone if they're not ready to be eaten i think it's ok initiator
{   
- cnetAnswer4==0:
    Add answer?
    -> answers ->
- cnetAnswer4==1:
    > i believe it is ok because you must have truly loved them and now you will be together forever which sounds nice.
- cnetAnswer4==2:
    > initiator i would try to refrain from doing this again unless you really want to which is difficult i understand.
- cnetAnswer4==3:
    > i'm sure initiator's relationship was full of complexities and beyond any floater's understanding this is a concept known as ambiguity
}
-> coralnetEnd

= answers
+   [Yes.]
    What will you say?
    ++  [It's ok.]
        -> coralnetAddedAnswer ->
        > i believe it is ok because you must have truly loved them and now you will be together forever which sounds nice.
        ~cnetAnswer4=1
        -> coralnetEnd
    ++  [It's not ok.]
        -> coralnetAddedAnswer ->
        > initiator i would try to refrain from doing this again unless you really want to which is difficult i understand.
        ~cnetAnswer4=2
        -> coralnetEnd
    ++  [It's in-between.]
        -> coralnetAddedAnswer ->
        > i'm sure initiator's relationship was full of complexities and beyond any floater's understanding this is a concept known as ambiguity
        ~cnetAnswer4=3
        -> coralnetEnd
+   [No.]
->->

VAR cnetAnswer5=0
=== coralnet5 ===
{coralnet5>1: 
    ~talkedToThisCoralnet=true
- else:
    ~talkedToThisCoralnet=false
}
-> coralnetStart ->
Coralnet: motif: does anyone yearn?
> does anyone yearn to be in the next season? i miss the parties
> initiator why dont' you just start your own party here
> it's not the same
> initiator i'm always satisfied with the current state this doesn't seem right maybe you hit your head really hard? have you seen an educated healer?
> no
{   
- cnetAnswer5==0:
    Add answer?
    -> answers ->
- cnetAnswer5==1:
    > i understand initiator i yearn as well but for something different. i'm not sure what exactly yet.
- cnetAnswer5==2:
    > that's odd why would you desire something that you cannot bring about initiator? i find this disturbing
- cnetAnswer5==3:
    > i hate parties there's too many persons and most of them do not really love you i could not synch with you initiator
}
-> coralnetEnd

= answers
+   [Yes.]
    What will you say?
    ++  [I understand.]
        -> coralnetAddedAnswer ->
        > i understand initiator i yearn as well but for something different. i'm not sure what exactly yet.
        ~cnetAnswer5=1
        -> coralnetEnd
    ++  [It's weird]
        -> coralnetAddedAnswer ->
        > that is odd why would you desire something that you cannot bring about initiator? i find this disturbing
        ~cnetAnswer5=2
        -> coralnetEnd
    ++  [I hate parties]
        -> coralnetAddedAnswer ->
        > i hate parties there's too many persons and most of them do not really love you i could not synch with you initiator
        ~cnetAnswer5=3
        -> coralnetEnd
+   [No.]
->->


VAR cnetAnswer6=0
=== coralnet6 ===
{coralnet6>1: 
    ~talkedToThisCoralnet=true
- else:
    ~talkedToThisCoralnet=false
}
-> coralnetStart ->
Coralnet: motif: what's it like outside?
> motif.
> i don't know
> i don't know
> i tried going but i felt very bad everywhere and had to come back inside.
> has anyone who's traversed come back yet?
> my sibling said they did and there was sharp glass everywhere and everything was blurry and they never felt hungry again
> but i think it was a lie
{   
- cnetAnswer6==0:
    Add answer?
    -> answers ->
- cnetAnswer6==1:
    > probably terrible for your skin etc. i do not recommend that people go it seems unsafe.
- cnetAnswer6==2:
    > so amazing and nice i'm sure and no one there misses us and they're dancing and holding each other close it fills me with bile
- cnetAnswer6==3:
    > i don't know i wish i knew but also not really does anyone perhaps understand the state i'm in?
}
-> coralnetEnd

= answers
+   [Yes.]
    What will you say?
    ++  [Probably bad.]
        -> coralnetAddedAnswer ->
        > probably terrible for your skin etc. i do not recommend that people go it seems unsafe.
        ~cnetAnswer6=1
        -> coralnetEnd
    ++  [Probably great.]
        -> coralnetAddedAnswer ->
        > so amazing and nice i'm sure and no one there misses us and they're dancing and holding each other close it fills me with bile
        ~cnetAnswer6=2
        -> coralnetEnd
    ++  [I don't know.]
        -> coralnetAddedAnswer ->
        > i don't know i wish i knew but also not really does anyone perhaps understand the state i'm in?
        ~cnetAnswer6=3
        -> coralnetEnd
+   [No.]
->->


VAR cnetAnswer7=0
=== coralnet7 ===
{coralnet7>1: 
    ~talkedToThisCoralnet=true
- else:
    ~talkedToThisCoralnet=false
}
-> coralnetStart ->
Coralnet: motif: i went outside
> i went outside and there was sharp glass everywhere and everything was blurry and i was never hungry
> that's not true my lover said they were born outside and they never mentioned glass
> what else did they say?
> they said the ground burns your feet and everything you try to eat turns into spikes so they had to come down here
> your head is full of planktons i went outside and it's the same as here but more magenta
{   
- cnetAnswer7==0:
    Add answer?
    -> answers ->
- cnetAnswer7==1:
    -> answer1 ->
- cnetAnswer7==2:
    -> answer2 ->
- cnetAnswer7==3:
    -> answer3 ->
}
-> coralnetEnd

= answers
+   [Yes.]
    What will you say?
    ++  [This is scary.]
        -> coralnetAddedAnswer ->
        -> answer1 ->
        ~cnetAnswer7=1
        -> coralnetEnd
    ++  [Seahorseshit.]
        -> coralnetAddedAnswer ->
        -> answer2 ->
        ~cnetAnswer7=2
        -> coralnetEnd
    ++  [Did you meet them?]
        -> coralnetAddedAnswer ->
        -> answer3 ->
        ~cnetAnswer7=3
        -> coralnetEnd
+   [No.]
->->

= answer1
> this is scary does anyone know if a regular person could survive and maybe return?
->->
= answer2
> come on there's no way it's that bad i'm sure it's just boring and stupid and empty and you die of dullness
->->
= answer3
> initiator i would like to know if perhaps you met a very august yet cold-eyed person and maybe how they are doing but it matters not very much
->->


VAR cnetAnswer8=0
=== coralnet8 ===
{coralnet8>1: 
    ~talkedToThisCoralnet=true
- else:
    ~talkedToThisCoralnet=false
}
-> coralnetStart ->
Coralnet: motif: i miss my lover
> i miss my lover a lot. what do i do?
> initiator did your lover provide you with most of your food? you could find a good hunter to replace them
> no it wasn't like that.
> i think i just really liked their company.
> initiator did they have a particularly good voice? maybe you could start going to more concerts and operas
> initiator i am good at finding and providing food for weak ones maybe we should connect
> i tried this (not food thing) but i still feel the missing.
> what do i do?
> what can i do?
> what should i do?
{   
- cnetAnswer8==0:
    Add answer?
    -> answers ->
- cnetAnswer8==1:
    -> answer1 ->
- cnetAnswer8==2:
    -> answer2 ->
- cnetAnswer8==3:
    -> answer3 ->
}
-> coralnetEnd

= answers
+   [Yes.]
    What will you say?
    ++  [Still looking.]
        -> coralnetAddedAnswer ->
        -> answer1 ->
        ~cnetAnswer8=1
        -> coralnetEnd
    ++  [Also wondering.]
        -> coralnetAddedAnswer ->
        -> answer2 ->
        ~cnetAnswer8=2
        -> coralnetEnd
    ++  [Pathetic.]
        -> coralnetAddedAnswer ->
        -> answer3 ->
        ~cnetAnswer8=3
        -> coralnetEnd
+   [No.]
->->

= answer1
> i am still looking for answers if anybody could provide them.
->->
= answer2
> i am not initiator but i am curious about this not because i feel similarly simply out of inquisitiveness.
->->
= answer3
> initiator you sound so pathetic it's actually pitiful why would you care so much what a floater you are
->->

VAR cnetAnswer9=0
=== coralnet9 ===
{coralnet9>1: 
    ~talkedToThisCoralnet=true
- else:
    ~talkedToThisCoralnet=false
}
-> coralnetStart ->
Coralnet: motif: i see them everywhere
> ever since my lover left me i see them everywhere i always feel their presence even though they're not there.
> initiator did you also sing about missing your lover in another coral?
> no? that wasn't me.
> initiator that sucks you should really just try living in the moment
> initator have you tried the entanglement yet? maybe it would fix you
> that sounds scary.
{   
- cnetAnswer9==0:
    Add answer?
    -> answers ->
- cnetAnswer9==1:
    -> answer1 ->
- cnetAnswer9==2:
    -> answer2 ->
- cnetAnswer9==3:
    -> answer3 ->
}
-> coralnetEnd

= answers
+   [Yes.]
    What will you say?
    ++  [How to get better?]
        -> coralnetAddedAnswer ->
        -> answer1 ->
        ~cnetAnswer9=1
        -> coralnetEnd
    ++  [I still see them.]
        -> coralnetAddedAnswer ->
        -> answer2 ->
        ~cnetAnswer9=2
        -> coralnetEnd
    ++  [Pitiful.]
        -> coralnetAddedAnswer ->
        -> answer3 ->
        ~cnetAnswer9=3
        -> coralnetEnd
+   [No.]
->->

= answer1
> does anyone know how one could get better just wondering for initiator if there's better suggestions that they haven't tried or are less stupid.
->->
= answer2
> i still see them and it hurts in a sort of painful way but it's not a big deal
->->
= answer3
> initiator you sound so pitiful it's actually pitiful why would you care so much what a washout.
->->

VAR cnetAnswer10=0
=== coralnet10 ===
{coralnet10>1: 
    ~talkedToThisCoralnet=true
- else:
    ~talkedToThisCoralnet=false
}
-> coralnetStart ->
Coralnet: motif: current order
> does anyone know why the current sometimes is really cold and sometimes too hot?
> i think it's when people emit evil and dark vibrations it makes it cold but sometimes hot and it's perfect when there's only nice people around
> i don't know
> what the second person said sounds right
{   
- cnetAnswer10==0:
    Add answer?
    -> answers ->
- cnetAnswer10==1:
    -> answer1 ->
- cnetAnswer10==2:
    -> answer2 ->
- cnetAnswer10==3:
    -> answer3 ->
}
-> coralnetEnd

= answers
+   [Yes.]
    What will you say?
    ++  [Second person.]
        -> coralnetAddedAnswer ->
        -> answer1 ->
        ~cnetAnswer10=1
        -> coralnetEnd
    ++  [Doesn't matter.]
        -> coralnetAddedAnswer ->
        -> answer2 ->
        ~cnetAnswer10=2
        -> coralnetEnd
    ++  [Emotions.]
        -> coralnetAddedAnswer ->
        -> answer3 ->
        ~cnetAnswer10=3
        -> coralnetEnd
+   [No.]
->->

= answer1
> i agree the second person was right i think.
->->
= answer2
> this is such a trivial thing to worry about, i envy you, initiator. you do not know true pain.
->->
= answer3
> i believe it is tied to emotions for example if someone you called a friend refuses to play with you and everything feels both big and small which is like unbearable coldness.
->->


VAR cnetAnswer11=0
=== coralnet11 ===
{coralnet11>1: 
    ~talkedToThisCoralnet=true
- else:
    ~talkedToThisCoralnet=false
}
-> coralnetStart ->
Coralnet: motif: has anyone tried this
> recently i tried wrapping my organ around of a pillar and it felt extremely good
> what organ? what are you talking about
> if you don't know what i'm talking about this is not meant for you
> initiator i tried this and i didn't feel anything did you do anything else?
> i just kind of rubbed it a lot and it was really great
> i did this too but instead of a pillar it was a wall and i spread it kind of everywhere
> me too i think i prefer than doing it with other people
> what are you all talking about???
{   
- cnetAnswer11==0:
    Add answer?
    -> answers ->
- cnetAnswer11==1:
    -> answer1 ->
- cnetAnswer11==2:
    -> answer2 ->
- cnetAnswer11==3:
    -> answer3 ->
}
-> coralnetEnd

= answers
+   [Yes.]
    What will you say?
    ++  [What?]
        -> coralnetAddedAnswer ->
        -> answer1 ->
        ~cnetAnswer11=1
        -> coralnetEnd
    ++  [Why?]
        -> coralnetAddedAnswer ->
        -> answer2 ->
        ~cnetAnswer11=2
        -> coralnetEnd
    ++  [Disgusting.]
        -> coralnetAddedAnswer ->
        -> answer3 ->
        ~cnetAnswer11=3
        -> coralnetEnd
+   [No.]
->->

= answer1
> what is this about?
->->
= answer2
> what was the point of this action? i feel an odd emotion as i hear this which makes me also quizzical.
->->
= answer3
> this puts me in a state of revulsion but i am not sure why. so i would like to learn more if possible.
->->


VAR cnetAnswer12=0
=== coralnet12 ===
{coralnet12>1: 
    ~talkedToThisCoralnet=true
- else:
    ~talkedToThisCoralnet=false
}
-> coralnetStart ->
Coralnet: motif: keeping the secret
> there's a lot of people who haven't experienced "it" yet and i don't like talking to them because they are like little fingerlings.
> how can we know who else has done it so we can avoid the unchanged?
> initiator are we keeping it a secret and why i wasn't aware of this
> i think those of us who had it were chosen and we shouldn't mix with the less-deserving
> chosen for what
> initator chosen by whom
> it doesn't matter can you just answer my question
> i try using a lot of euphemisms... "it" is a good one. i'm also partial to "relations" and "enjoyment"
> has anyone come up with an actual name for it actually??
> what are we talking about???
{   
- cnetAnswer12==0:
    Add answer?
    -> answers ->
- cnetAnswer12==1:
    -> answer1 ->
- cnetAnswer12==2:
    -> answer2 ->
- cnetAnswer12==3:
    -> answer3 ->
}
-> coralnetEnd

= answers
+   [Yes.]
    What will you say?
    ++  [What?]
        -> coralnetAddedAnswer ->
        -> answer1 ->
        ~cnetAnswer12=1
        -> coralnetEnd
    ++  [Stop.]
        -> coralnetAddedAnswer ->
        -> answer2 ->
        ~cnetAnswer12=2
        -> coralnetEnd
    ++  [I know too.]
        -> coralnetAddedAnswer ->
        -> answer3 ->
        ~cnetAnswer12=3
        -> coralnetEnd
+   [No.]
->->

= answer1
> what is this about? i would love to learn
->->
= answer2
> please stop using the coralnet for unfair conversations that leave a lot of us out it doesn't feel nice.
->->
= answer3
> i am also aware of the subject of this conversation and i am so glad i am not part of the lesser non-knowers and such.
->->

=== coralnetProgress === //the coralnet to read to progress story
-> coralnetStart ->
// SOUND CUE: OST FOR THIS
Coralnet: motif: my entanglement
~ stopInstance("Underwater Ambiance",false)
~ fadeOut(-1)
> i experienced it today. i'm happy to share my experience.
> oh? how was it? tell us about it.
~ playInstance("event:/Ambience/mainActAmbiance","Main Act Ambiance",1)
~ setInstanceParameter("Main Act Ambiance", "intensity", -1, true)
> it was in the back of our music hall. 
> we were practicing a song for the end-of-season.
~ setInstanceParameter("Main Act Ambiance", "intensity", 0, false)
> we felt a tugging from underneath our skin, in the middle top of our back.
> who did you do it with?
> it was my singing partner. not my lover, but someone i've always been deeply intimate with.
> and after the tugging?
~ setInstanceParameter("Main Act Ambiance", "intensity", 1, false)
> first, there was a burning sensation. 
> then it started to come out.. and it felt cold as ice.
~ setInstanceParameter("Main Act Ambiance", "intensity", 2, false)
> what was it exactly?
> it was neither tail nor fin nor antenna. it was a tendril of sorts. 
~ setInstanceParameter("Main Act Ambiance", "intensity", 3, false)
> it was long, and sinewy, and soft.

> there was a beautiful burst of smaller wriggling tendrils at the end of it, like a flower.
> and what did you do with it?
~ setInstanceParameter("Main Act Ambiance", "intensity", 4, false)
> at first we merely sensed our surroundings
> the organ felt even more sensitive than the inside of my mouth. which is very sensitive.
> then we started feeling each other. we brushed ourselves and we made knots of our bodies.
~ setInstanceParameter("Main Act Ambiance", "intensity", 5, false)
> we tugged and let go. we became one with the current.
> we became stickier. it was more difficult to separate each other.
> our organs were the same color and we couldn't tell which was whose
~ setInstanceParameter("Main Act Ambiance", "intensity", 6, false)
> and how did it feel?
> initiator, how did it feel? was it pleasurable?
> i wish this happened to me
> initiator, tell us how it was.
~ setInstanceParameter("Main Act Ambiance", "intensity", 7, false)
> it sounds amazing.
> initiator? 
> this should have been me
~ stopInstance("Main Act Ambiance", true)
> would you do it again? initiator?
~pauseTutorial(false)   //coralnetEnd stuff
{ npcsTalkedTo > 2:
    ~finishTutorialPart(11)
}
//~ continueSinging()
~ restoreNPCsVolume()
~ switchObject("Teacher - Library",true)
~ makeInterlocutorIdle()
~ switchInterlocutor("Teacher - Library")
~ activateBorder("coral",false)
~ playInstance("event:/Ambience/underwaterAmbiance","Underwater Ambiance",1)
-> teacherAtLibrary

// I like this one very much. I like the recurring reference to how it feels inside one's mouth. Also imagining a world where this part could feel more cut off vibe since the teacher is probably approaching in the middle when MC is reading? Now is also very nice!


/* MAIN STORY */

// SOUND CUE: ADD QUICK TEACHER BARK SOUND
// SOUND CUE: SOUND WHEN STARTLED
VAR awkwardnessLevel=3
// Initiated by the teacher as you finish reading coralnetProgress
=== teacherAtLibrary ===
 -> npcStart1 ->

~ stopSinging()
~ switchObject("Roadblock - Library",false)
~ switchObject("Coral - Library",true)
~ libraryOpen=true
~ fadeIn(1)
~ playOneShot("event:/Non-Diagetic SFX/Wipe With Note - Short",0.5)
Teacher: Sounds amazing, doesn't it? #speed:fast
MC: Huh?
Teacher: The entanglement. \\pauseYou were reading about it just now, right? Have you done it yet?
+   [How did you connect with me?]
    MC: How did you connect with me?
    Teacher: Oh! Sorry!! I must've startled you..
    Ah.. you were humming to yourself. I harmonized with you.
    Sorry, I guess maybe you weren't fully aware. I know it's not usual to talk at these sort of places..
    ++  [It's fine.]
        MC: It's fine. Uhm..
        I guess I was engrossed. But it's nice to.. discuss, and stuff.
        Teacher: Yeah, some of the things they sing about can get pretty weird, right? Haha.
        MC: ..I guess so.
        You were saying something about the, um, entanglement?
        Teacher: Oh! Yeah..
        ~awkwardnessLevel=awkwardnessLevel-1
    ++  [It's weird.]
        MC: Yeah, I kind of prefer just quietly reading, right now.
        Teacher: Ah...
        Um.. I'll leave you alone then.
        MC: You were saying something, about this.. entanglement?
        I guess I'm uh.. kind of curious
        Teacher: Oh! Yeah.
        ~awkwardnessLevel=awkwardnessLevel+1
+   [I haven't.]
    MC: No, I haven't.
    I'd like to hear more.
    Teacher: A-ah?
    MC: I mean, uh.. if you know anything about it?
    Teacher: Yeah.. Actually...
    ~awkwardnessLevel=awkwardnessLevel-1
- Teacher: I've done it a couple times now. 
It wasn't nearly as intense as they described, but maybe.. I just haven't found the right person?
Teacher: I wonder what happens at the end, maybe some sort of a giant sparkly explosion? Haha..
MC: But you're saying you've done it before...?
Teacher: Oh, yes, right. I have. 
But I've never reached this "climax" that I've often heard of. I've always stopped before. Both times I've done it.
+   [Why?]
    MC: Why?
    Teacher: It always felt a bit scary. \\pause\\pauseLike maybe I would burst.
    ~pause(2)
    But also...
    ++  [Exciting?]
        MC: Exciting?
        Teacher: Yes.
        ~ awkwardnessLevel=awkwardnessLevel-1
        ~ pause(2)
    ++  [Also?]
        MC: Also?
        Teacher: Um.. exciting, I guess.
        ~ awkwardnessLevel=awkwardnessLevel+1
        ~ pause(2)
+   [You should keep going.]
    MC: You should try to keep going. It might be interesting.
    Teacher: Ah.. yes, perhaps.. 
    I think I just haven't found someone that I feel.. completely comfortable with.
    ~awkwardnessLevel=awkwardnessLevel-1
    ~ pause(2)
    MC: I see.
+   [I understand.]
    MC: I understand. 
    Teacher: Ah?
    ++  [It's probably scary]
        MC: I'm sure it's... frightening.
        Teacher: Yes, but also...
        Exciting.
        MC: Ah.
        ~awkwardnessLevel=awkwardnessLevel-1
    ++  [Yes.]
        MC: Yes.
        ~awkwardnessLevel=awkwardnessLevel+1
- Teacher: ...
Teacher: I always see you here. Do you only come to browse or...?
MC: Uh... Well I didn't come here to...
Teacher: To..?
MC: I mean, uh, I like to read.
Teacher: Yes! Me too.
Sorry, I didn't mean to ask if you were here looking for..
MC: Right. I didn't think you were either.
~ pause(2)
Teacher: Say, it looks like the current is letting up. I could use a bite. Perhaps we can continue this conversation at the diner?
~ loadInt("showcaseMode")
{ showcaseMode==1:
    -> skipToTheEnd ->
- else:
    -> normalEnd ->
}
-> END

= normalEnd
MC: Oh. You want to.. uh..
~ pause(2)
MC: Actually..
+   [No.]
    MC: No. Sorry. 
    ~ overrideRotationWithSpeed("Roadblock - Library",1.5)
    ~ pause(2)
    I really need to get going.
+   [I don't know.]
    MC: I don't know, I--
    ~ overrideRotationWithSpeed("Roadblock - Library",1.5)
    ~ pause(2)
    I really should get going.
+   [I have other things to do.]
    MC: Sorry, I am... doing other things. 
    ~ overrideRotationWithSpeed("Roadblock - Library",1.5)
    ~ pause(2)
    Not at the diner.
- ~ pauseTutorial(false)
~ restoreNPCsVolume()
~ desireStep=2
~ changeDesire("<br><br>Exit<br> Library.")
~ activateBorder("floral",false)
~ continueSinging()
->->

= skipToTheEnd
~ pause(2)
MC: Actually...
Why don't we just get straight to it?
Teacher: ...Ah?
MC: We don't have much time, and this is a showcase demo after all. So let's skip the boring fluff...
MC: Do you know anywhere with less people?
Teacher: ..Hm. This is quick.
But I guess you're right. This would normally be a slower burn but there's other gameplay modes to see.
I know a place. Follow me.
- ~ loadLevel("Foreplay 1 - 1")
~ activateBorder("floral",false)
->->

// AT THIS POINT MAKE TEACHER RESPOND TO SINGING

VAR talkedToTeacherAgain=false

=== teacherAtLibrary2 ===
-> npcStart1 ->
Teacher: Did you change your mind about the diner?
MC: Um.. Well I don't know.
How do you get there again?
Teacher: It's through that exit, over there.
~ overrideRotationWithSpeed("Roadblock - Library",1.5)
~ pause(2)
It was blocked earlier, but seems like the current's let up.
MC: Ok.
+   [See you there.]
    MC: See you there.
    Teacher: Ah!
    I.. look forward to it.
    { talkedToTeacherAgain == false:
        ~awkwardnessLevel=-1
    }
+   [Cool.]
    MC: Cool. I was just wondering.
    Teacher: Okay..
    { talkedToTeacherAgain == false && awkwardnessLevel>-1:
        ~awkwardnessLevel=awkwardnessLevel-1
    }
- ~talkedToTeacherAgain=true
- ~ changeStartKnot("teacherAtLibrary3")
-> npcEnd ->
-> END

=== teacherAtLibrary3 ===
-> npcStart1 ->
MC: It looks like this coralnet stopped working..
Teacher: Oh? Yes, it seems as much.
I've heard that they can get short-circuited by intense emotions..
MC: Intense emotions..?
Teacher: Maybe.. when I startled you?
MC: Ah.. I guess so.
~ pause(2)
Teacher: So you came to read it again?
MC: Uh.. I just wanted to.. add a reply..
Teacher: What would you have said?
+   [Seems exciting.]
    MC: That it seemed like an exciting experience.
    ~pause(2)
    Teacher: Would you like to try it?
    ~ pause(2)
    MC: Perhaps.
+   [Asked how it was.]
    MC: I guess I would've also asked what it felt like.
    Teacher: A lot of others already asked. Wouldn't that be redundant?
    MC: Um.. Can't hurt.
+   [A joke.]
    MC: Some sort of a joke.. maybe.
    Teacher: Oh? Like what?
    MC: Uhm..
    ~pause(2)
    It seems to evade me now.
    Teacher: Ah.
- ~ changeStartKnot("teacherAtLibrary2")
{ awkwardnessLevel>-1:
    ~awkwardnessLevel=awkwardnessLevel-1
}
-> npcEnd ->
-> END



// MC finds teacher at diner sitting. They start singing when MC gets nearby to invite conversation
=== teacherAtDiner ===
 -> npcStart1 ->
{ 
- talkedToTeacherAtDiner:
    Teacher: <>
    {stopping:
        - Did you change your mind?
        - Are you really sure you don't wanna go?
        - Are you really really sure you don't wanna go? 
        - ...
    }
    Well?
    +   [Let's go.]
        MC: Yes. Let's go now.
        -> going
    +   [No.]
        MC: No.
        Teacher: ...Okay then.
        -> npcEnd ->
        -> END
- else:
    ->chat->
    -> END
}

= chat
~talkedToTeacherAtDiner=true
{ 
 - talkedToTeacherAgain:
    Teacher: So you came.
    MC: Yes.
    ~pause(4)
    { awkwardnessLevel>1:
        Teacher: I'm surprised. I thought I made a bad impression.
        MC: It's.. It was fine. I was just startled.
        ~ pause(2)
        MC: But I wanted to come here.
    }
 - awkwardnessLevel<1:
    Teacher: I had a feeling you'd come.
    MC: But I said I wouldn't.
    Teacher: Oh, haha.
    MC: Why would you think I would?
    Teacher: Well.. Um. Just a vibe I got.
    ~ pause(4)
 - awkwardnessLevel>3:
    Teacher: Oh, you're here!
    MC: Yeah.
    ~pause(2)
    Teacher: I must've been really bothersome at the library. #speed:fast # ambient # time:0.5
    MC: I was really curt with you. # speed:fast # time:0.5 # notambient
    Teacher: It's alright.
    ~pause(4)
    So what brings you here?
 -  else:
    Teacher: I'm surprised you've come after all.
    MC: Yeah..
    I think I am as well.
    ~ pause(2)
}
Teacher: Can I share some food with you? They have really tasteful salmonds this season.
MC: I'm not hungry.
~pause(4)
MC: Have you been watching me?
Teacher: What?
MC: At the library. You said that you've often seen me there.
Teacher: Hmm.. Well.. I wouldn't say I watch you..
I just meant.. I notice you there almost everytime I visit.
MC: Right.
~pause(2)
MC: ...
+   [I'm noticeable.]
    MC: I guess I'm noticeable.
    Teacher: I would say so, yes.
+   [(Say nothing.)]
- ~ pause(2)
Teacher: Would that be bad? If I watched you?
+   [No.]
    MC: No.
    Teacher: Then I guess I do watch you a little bit.
+   [I would like it.]
    MC: I would actually like it.
    Teacher: Then I guess I do watch you a little bit.
+   [Yes.]
    MC: Yeah, kind of.
    ~ pause(2)
    It's weird when you don't know someone like that.
    Teacher: Ah... I guess so.
    MC: But we know each other now.
    Teacher: We do.
- ~ pause(4)
Teacher: Since you're not hungry.. Would you like to..
MC: Yeah?
~ changeDesire("<br>Follow <br>the library stranger.")
~ desireStep=4
Teacher: Go somewhere with fewer people?
+   [Let's go.]
    MC: Sure. Let's go.
    -> going
+   [No.]
    MC: Oh. Uhm...
    Not really.
    Teacher: Ah. Okay.
    ~pause(2)
    Let me know if you change your mind.
    -> npcEnd ->
- ->->

= going
Teacher: Okay then. I'll show you one of my favorite places.
~ fadeOut(1) //maybe at this point show a camera view of the tunnel?
You know the tunnel entrance, when you exit the diner and head straight to the bottom?
Meet me there.
MC: Wait, what about your food...?
Teacher: Oh..
It's fine. Others will eat it.
~ activateBorder("floral",false)
~ changeDesire("<br>Meet <br>the library stranger at the bottom.")
~ restoreNPCsVolume()
~ pauseTutorial(false)
~ switchObject("Teacher",false)
~ switchObject("Teacher - Center",true)
~ fadeIn(1)
-> END

// SOUND CUE: TEACHER LEAVING

=== teacherAtCenter ===
-> npcStart1 ->
Teacher: I know a trick for getting rid of these.
~fadeOut(1)
Let's see... I'm focusing my psychic powers...
~ pause(2)
// SOUND CUE: OPENING PORTAL (MAYBE ALSO IN LIBRARY)
~ switchObject("Coral - Edge Tunnel",true)
~ switchObject("Roadblock - Edge",false)
~ followingTeacher=true
There we go.
~ fadeIn(1)
MC: Where did you learn to do that?
Teacher: Um.. One of my past lovers.
MC: Does the entanglement make you..?
Teacher: No, it also depends on your diet. You should incorporate more rock salt.
Anyway, let's go.
~changeDesire("<br>Follow<br> the library stranger.")
~ nextBrain()
~ restoreNPCsVolume()
~ pauseTutorial(false)
~ activateBorder("floral",false)
-> END


// MC follows teacher in gameplay portion to edge 2


//Appears if harmonizing while on the way to the edge
=== teacherOnTheWay1 ===
# ambient
~ activateBorder("floral",true)
~ stopSinging()
{->one->|->two->|->three->|->four->|->five->}
~ activateBorder("floral",false)
~ continueSinging()
-> END

= one
Teacher: Are you excited? # time: 2
~ pause(1)
MC: I think. # time: 2
~ pause(2)
Teacher: Me too. #time: 2
~ activateBorder("floral",false)
->->
= two
MC: Have you done this often? # time: 3
Teacher: Only a couple times... # time: 3
Teacher: Don't worry, it's not scary or painful. # time: 3
Teacher: I mean, not very scary. Maybe a bit. # time: 3
->->
= three 
Teacher: I'm a bit full from the salmonds. # time : 3
And the chestnurchins. # time : 3
MC: Did you try the mackeroni? # time : 3
Teacher: It's ok. I prefer the turtliatelle. # time: 3
->->
= four
MC: Is it ok if you're so full? # time: 3
MC: I heard you have to wait five currents after a meal before trying to entangle # time: 5
Teacher: Is that what we're doing? # time: 3
MC: Um. Are we-- I thought-- # time: 3
Teacher: I'm kidding. I'm sure it's fine. Maybe it helps you digest. # time : 4
->->
= five
MC: How much farther away? # time: 3
Teacher: Just a few more strokes... # time: 3
->->


//TO-DO: add more things that they can say on the way.

=== teacherArrivedAtEdge ===
~ switchObject("LevelLoader - Foreplay 1",true)
~ nextBrain()
->END

// MC and Teacher harmonize then sit and talk
=== teacherAtEdge1 ===
~stopSinging()
Teacher: It's nice here, isn't it?
MC: Yeah. I was actually here just now.
Teacher: Oh, haha, wait, really?
MC: Yeah.
Teacher: That makes sense. I can picture you hanging out here.
~pause(2)
Teacher: Do you usually come by -- #speed:fast # ambient # time:0.25
MC: So do you want to try it with me?  #speed: fast  # time:0.5 # notambient
Teacher: Oh!
Teacher: Yes, sure. \\pause\\pauseI mean, that's what I thought this was leading to.
MC: Oh. Sorry. Is talking about it directly.. abnormal?
Teacher: No, not at all!
...
Teacher: I feel really attracted to you.
+   [Thanks.]
    MC: Ok. Thanks.
    Teacher: ...
+   [Me too.]
    MC: Yeah, me too.
    Teacher: That's good to know, haha.
    Teacher: I mean, I thought so.
+   [I know.]
    MC: Yeah. I could tell. I guess.
    MC: That's cool.
    Teacher: ...Right. You're welcome.
- MC: How do we get started?
Teacher: Ah...\\pause\\pause Well that's the thing. It's been different everytime I've done it. 
Teacher: It would just kind of happen spontaneously. But maybe...
MC: Yes?
Teacher: Maybe we should try just harmonizing for a while? Like they say on the coralnet?
~ loadInt("showcaseMode")
{ showcaseMode==1:
    MC: What about.. what do we do when the organs are out?
    Teacher: I think we can...\\pause just try to wrap around each other?
}
MC: Ok. Let's do it.
Teacher: You can take the lead.
~ nextBrain()
-> END
// Teacher and MC harmonize a few times

// Maybe add a "I was here just now" vibe on line 249?
// Love the interaction starting at line 254


//After harmonizing a few times
=== teacherAtEdge2 ===
MC: I can feel the... 
Teacher: Yes, me too.
MC: Should we...?
Teacher: Let's keep going.
~nextBrain()
-> END
// Teacher and MC harmonize a few more times, then main act starts

=== teacherAtEdge3 ===
~nextBrain()
~ loadInt("showcaseMode")
{ showcaseMode==1:
    ~loadLevel("Main Act 1 - 2")
- else:
    ~loadLevel("Main Act 1 - 1")
}
-> END

VAR freezeMoment=false

//Right after the organs come out
=== teacherMainAct1 ===
# ambient
~ freezeMoment=true
~ switchObject("Overlay",true)
MC: WAIT, stop!!! # time: 3 # speed:fast
-> END
// Main act interrupted and go back to edge sitting

=== teacherAtEdge4 ===
~stopSinging()
MC: Sorry, \\pausethat was... #speed: slow
Teacher: Are you okay? #speed: slow
MC: Yeah, \\pauseI guess I just didn't expect it to feel that way.
Teacher: Did it hurt?
MC: No.
MC: It was just something I've never felt before.
~pause(2)
Teacher: Right.
~pause(6)
MC: What do we do when the organs are out?
Teacher: I think we can...\\pause just try to wrap around each other?
~pause(2)
Teacher: Do you wanna stop for now?
MC: No I want to keep going. #speed: fast
Teacher: Ok.
Teacher: We can start whenever you feel comfortable. 
Let's just harmonize again when you're ready.
MC: Ok.
~nextBrain()
-> END
// Starts again after harmonizing just once

=== teacherAtEdge5 ===
~nextBrain()
~loadLevel("Main Act 1 - 2")
-> END


VAR blink=false

//We could definitely cut parts of this if it's too long/too much to program, I tried to give as much choice opportunities as possible
// I feel like we can definitely see after playtest. But I think we can keep it now its all good stuff to me
=== teacherCuddling ===
~ loadInt("sexIntensity")
~pause(4)
~ changeDialogueView(1)
Teacher: So... #speed: slow
~ finishTutorialPart(1)
Teacher: How was that?
+   [Good.]
    ~ finishTutorialPart(2)
    MC: Good. \\pauseIt felt really good. \\pauseAll over my body.
    Teacher: Really?
    MC: Yeah.
+   [Weird.]
    ~ finishTutorialPart(2)
    MC: Kind of weird. \\pauseI'm not sure yet how I feel about it.
    Teacher: But you don't... Do you regret it?
    MC: No, no.
    MC: I'm glad we did it. 
    MC: I think.
    Teacher: Me too.
+   [I'm not sure.]
    ~ finishTutorialPart(2)
    MC: I'm not sure.
    MC: I mean, the physical sensations were amazing. \\pauseMaybe the best I've ever felt.
    MC: But also... I don't know.
- MC: What about you?
{
    - sexIntensity<3:
        Teacher: It was kind of mellow. More than most I've done.
        Teacher: But I liked it. It felt also more intimate than most.
    - else:
        Teacher: It was very intense, wasn't it?
        Teacher: I enjoyed it very much.
}
Teacher: I really liked the way you felt.
Teacher: And that climax... Did you expect it to just fall off like that?
+   [Yeah.]
    MC: It felt kind of natural.. to me. Like it was leading up to it? It made sense.
    Teacher: Maybe.
     MC: Yeah.
+   [No.]
    MC: No! Does that mean we can't do it again?
    Teacher: Haha, no, I think it grows back.
    MC: I see.
- ~ changeDialogueView(4)
MC: By the way..
Teacher: Yes?
MC: What attracted you to me?
Teacher: Oh. Hmm...
You have a certain... sullen air.
MC: Sullen?
Teacher: Yes. Broody.
MC: ...
Teacher: I guess I found it mysterious.
MC: ...
Teacher: What about you?
+   [Your straight- forwardness.]
    MC: I kind of like that you came up to me so directly.
    MC: It made things easy.
    Teacher: Oh, yes. 
    ~pause(2)
    I guess I'm that way.
    ~ changeDialogueView(2)
    Anyway...
+   [Your experience.]
    MC: I liked that you're more experienced than me.
    Teacher: Ha...
    ~pause(2)
    Not by that much.
    ~ changeDialogueView(2)
    Speaking of..
+   [Your physical body.]
    MC: I like the way that you look.
    Like.. more than I do.. most others. I guess.
    Teacher: Ah. 
    ~pause(2)
    I see.
    ~ changeDialogueView(2)
    Anyway...
- Teacher: You said you've never done this before, right?
+   [I haven't.]
    MC: No, I haven't.
    Teacher: Not even...
+   [Why do you ask?]
    MC: Why are you asking that?
    Teacher: Just curious.
    Teacher: What about...
- With your lover? Before they left?
MC: How do you know about them?
Teacher: Oh! I mean...
Teacher: Like I said, I've seen you around. And people talk.
~ changeDialogueView(3)
Teacher: And...
MC: ?
Teacher: You sung about them, right? I think I read it on the coralnet, one time.
+   [No.]
    MC: I don't know what you're talking about. Must've been someone else.
    Teacher: Ah. Well...
    MC: But yes, I mean...
+   [Yes.]
    MC: Yes. It's kind of embarassing that you mention it.
    Teacher: Ah-- I didn't mean to.
    Teacher: I guess those things are anonymous for a reason.
    MC: Yes. But yeah...
-MC: We never did. It wasn't...
~ changeDialogueView(4)
MC: I didn't know about it yet, back then. I think it was just starting to happen.
Teacher: Right.
Teacher: Would you have liked to... with them?
+   [Yes.]
    MC: Well, yes.
    MC: I mean... yeah. Probably.
    MC: But there's really no point in thinking about it. At this point.
    Teacher: Right...
+   [No.]
    MC: No. In fact I'd rather not think about it.
    Teacher: That makes sense. Sorry for bringing that up.
    MC: It's fine.
+   [I don't know.]
    MC: I don't know. Maybe. Probably. Maybe not. It's hard to visualize.
    MC: I'd really rather not think about it.
    Teacher: Right... Sorry for bringing that up.
- Teacher: But, you know... \\pause\\pause\\pausethings could be different once we're up there.
~ retractHandTrigger=true
MC: What?
MC: What do you mean?
~ blink=true
~ fadeOut(-1)
Teacher: Well you know... 
Teacher: They're saying that we're all starting to change so we can live in the outside.
Teacher: And since we've entangled... We must be changing too.
MC: I don't-- What?
//Maybe transition to overworld view at this point
MC: Why would I want that? That's not why we did this. I--
MC: Is that really how it works?
Teacher: Oh? Um, I mean...?
Teacher: I'm not sure? It's just what I've heard. 
MC: Is that why you were trying to do it with me?
Teacher: What? No! I told you, I'm really attracted to you.
Teacher: And you asked first! I wasn't--
MC: I know. Um..
MC: I didn't know it was like that. I don't want to change. I can't-- #speed: fast
~ pause(2)
MC: I'm sorry. I think I need to get going. This was, um..
Teacher: No, I'm sorry. I-- #speed: fast
MC: Let's never do this again. # speed: fast
~ loadLevel("Migration 1")
-> END
// Transition to migration




/* RANDOM NPCS */

VAR talkedToReceptionist=false

=== libraryReceptionist ===
# color: 95B79B
{ npcsTalkedTo==0:
    ~npcsTalkedTo=npcsTalkedTo+1
}
-> npcStart1 ->
{ talkedToReceptionist==false && npcsTalkedTo<=2:
    ~pauseTutorial(false)
    ~goToTutorialPart(0)
}
NPC: Welcome to the library.
{ talkedToReceptionist==false && npcsTalkedTo<=2:
    ~goToTutorialPart(1)
}
Let me know if you need any help.
+   [What is this place?]
    ~pauseTutorial(true)
    This is the entrance to the library. 
    If you head further in, you can find a lot of the community's coralnet.
    Feel free to read or speak into any of them!
    ...By the way, do you know what coralnet is?
    ++  [Yes.]
        MC: Of course I know what coralnet is. I'm always here reading.
        NPC: Ok, just making sure!
    ++  [No.]
        MC: What is coralnet?
        NPC: Coralnet are those noisy brown conical things...
        ~ overrideRotationWithSpeed("Coralnet - Entrance",1.5)
        Like the one over there!
        People use them to deposit their thoughts and such.
        It uses coral technology. You know. Or something like that.
        You should try talking to one of them!
        ...but you knew all of this, didn't you? I've seen you here before!
        MC: Um...
        NPC: It's okay! I know why you asked!
        You know that I love to help! Thank you giving me the opportunity to explain. It's very sweet!
        MC: Ah.. No problem.
        NPC: Go on, I know how much you treasure your reading time. Thanks for singing with me!
        MC: I don't read.. that much.
+   [Who are you?]
    ~pauseTutorial(true)
    My name is Elevide! Nice to meet you.
    MC: Name..?
    NPC: It's nice that you came to talk to me today.
    ++  [Do you work here?]
        NPC: Oh, no. 
        I don't think anyone "works" here?
        I just really like helping people.
        If you want you can join me too. It's fun!
        MC: Maybe some other time...
    ++  [No worries.]
        MC: Sure, uh, no worries.
+   [I'm good thanks.]
    ~pauseTutorial(true)
    NPC: No troubles.
- ~pauseTutorial(false)
{ npcsTalkedTo > 2:
    ~ finishTutorialPart(11)
}
~continueSinging()
{
    - desireStep==0:
        ~desireStep=1
        ~changeDesire("<br>Read Coralnet.")
}
- -> npcEnd ->
{ talkedToReceptionist==false && npcsTalkedTo<=2:
    ~goToTutorialPart(10)
}
~ talkedToReceptionist=true
-> END

=== npcAtLibrary1 ===
# color: 7E0D13
-> npcStart1 ->
NPC: I LOVE SINGING INTO THE CORALNET 
{->one->|->two->}
 -> npcEnd ->
-> END
= one
I FEEL FREE TO EXPRESS MY DEEPEST MOST EMBARASSING SECRETS THAT I WOULD NEVER TELL ANYONE
LIKE HOW I WANTED TO END MY LAST ROMANTIC RELATIONSHIP BUT I COULDN'T BRING MYSELF TO GO THROUGH WITH IT
SO I COVERED MYSELF IN SAND AND I ATTACHED SCALES ON MY BODY AND PRETENDED TO BE A BURROWING FISH 
UNTIL MY EX LEFT ME ALONE
BUT THEN I REALLY ENJOYED THE BOTTOM-FEEDER LIFESTYLE AND I BECAME ADDICTED TO BURROWING 
AND EATING ANYTHING THAT FELL INTO MY MOUTH 
MOSTLY SAND ETC.
I WAS SO WRAPPED UP IN IT THAT I DIDN'T REALIZE EVERYONE HAD MOVED ON TO THE NEXT SEASON
SO I JUST STAYED THERE BUT THEN I MISSED ANOTHER SEASON 
THEN ANOTHER
AND NOW I THINK IT'S BEEN A FEW AND I THINK I JUST MET MY EX'S GRANDCHILD
MC: ...And you're just telling me this?
NPC: YOU SEEM REALLY TRUSTWORTHY
->->
=two
MC: That's good.
->->



=== npcAtLibrary2 ===
# color: 2b6136
-> npcStart1 ->
NPC: I really should stop coming to the library... 
The current in this corridor always ends up trapping me and I have to wait an ETERNITY before being able to go anywhere else.
NPC: Ah well, I guess I can catch up on some epics...
Did you hear the one about how they got their tail stuck in between two copulating clams?
+ [Yes.]
    NPC: Isn't it riveting? I'm eager to hear if how and when they got out.
    Say, have you checked it recently? Have they gotten out yet?
    No, actually don't tell me! I must find out for myself.
+ [No.]
    NPC: I highly recommend it! It is simply riveting.
    A true tale of patience and woe...
- -> npcEnd ->
-> END

=== npcAtLibrary3 ===
# color: 1F7A6E
-> npcStart1 ->
NPC: I hear that the pink coral is supposed to pacify water currents, but this entryway is almost always blocked...
I wonder if it is a ploy to get us to read more coralnet...
Of course, no one would profit materially so the ploymasters must be highly attention-seeking initiators seeking more audience...
-> npcEnd ->
-> END

=== npcAtLibrary4 ===
#color: 99AFAA
-> npcStart1 ->
{ libraryOpen==false:
    NPC: Hnnnmnghh... If I focus my psychic energy towards the pink coral... Maybe it will start working again...
    Please don't distract me... I know I can do this...
- else:
    NPC: I did it! I reactivated the pink coral using my fierce psychic control!
    It was all me! Pretty impressive right? Wait you didn't see it?
    Just wait here... I can shut it off then on once more so then you can believe me! Just give me one moment...
}
-> npcEnd ->
-> END

=== npcInCenter1 ===    //Eelor
# color: 6D6787
-> npcStart1 ->
NPC: I love kicking off walls! It's my favorite part about swimming!
Do you know how to do it?
+   [Yes.]
    Awesome! So do I!
+   [No.]
    Oh!
    Would you like me to teach you how?!
    ++  [Sure.]
        Ok!
        All you have to do is kick your feet against it!
        +++ [...What?]
            Do you need more instructions?
            You basically just have to rapidly swim away from a wall while being near one.
            Does that make more sense?
            ++++ [Yes.]
                Good!
            ++++ [No.]
                Oh!
                Basically, you need to do a "rapid button press".
                And it only works if you're swimming forward or backward.
                Surely that explains it?
                +++++   [Yes.]
                    Yeah, I figured it would.
                +++++ [No.]
                    Oh!
                    I don't think there's any other I could help you! It's not that hard.
        +++ [Oh, duh.]
            Yeah! Ask me again if you forget!
    ++  [No thanks.]
        Aw! Too bad!
-  -> npcEnd ->
-> END

=== npcInCenter2 ===    //(Fonsh)
#color: 99AFAA
-> npcStart1 ->
NPC: Do you know the secret to swimming really fast?
+   [Yes.]
    What is it then?
    ++  [A protein-rich diet.]
        What's a protein?
    ++  [Resoluteness.]
        Oh wait... really?
    ++  [An aerodynamic body.]
        I heard that before... Do you think mine is okay? I think my back's sort of sticking out..
+   [No.]
    Well do you wanna hear it?
    ++  [Yes.]
        The secret is to keep striding forward frequently, but not so rapidly that you get tired out!
        And in-between, keep pressing onwards so as not to lose your momentum!
        Pretty helpful right?
        +++ [Totally.]
            Yeah, you're welcome!
        +++ [Not really.]
            Oh? Well...
            How about this: bouncing off walls not only gives you a boost, it also preserves your momentum!
            How about that?
            ++++ [Great.]
                You're welcome.
            ++++ [Whatever.]
                Ok.
    ++  [No.]
        Uhm... okay.
-  -> npcEnd ->
-> END

=== npcInCenter3 ===
# color: 1d1c29
-> npcStart1 ->
NPC: A strange phenomenon occurs, whenever I attempt to stray too far from this environment.
NPC: It's as if there are invisible walls block my passage.
Some say it's the current, but I know the truth.
It's gigantic and powerful psychics who are holding us in place, because they are still preparing the next place for our arrival.
What do you believe?
+   [It's just the current.]
    Foolish deep sea slug. Remain blind.
+   [It's psychics.]
    Yes!
+   [Mindset.]
    NPC: Hmmm...
    So you're saying our mindset is keeping us here and unconsciously stopping us from moving forth? I see...
    I need to improve myself..
-  -> npcEnd ->
->END

=== npcInCenter4 ===
# color: 2b6136
-> npcStart1 ->
NPC: Ahem. Do you mind?
-> npcEnd ->
->END

=== npcInCenter5 ===    //Eelor
# color: 6D6787
-> npcStart1 ->
NPC: I'm just standing next to the hole, no big deal.
-> npcEnd ->
-> END

=== npcInCenter6 ===  //(Fonsh)
#color: 99AFAA
-> npcStart1 ->
NPC: When I am swimming around this column I am filled with... a special feeling.
NPC: I have seen others do certain dastardly things with it.. I wonder if I could as well.
-> npcEnd ->
-> END



// Chat with virgin friend to try to establish more info about MC
=== npcInCenter7 ===
# color: ffae1e
-> npcStart1 ->
NPC: Good tidings friend.
How's your current?
+   [Chilly.]
    NPC: I think it's been getting colder, hasn't it?
+   [Just right.]
    NPC: Really?
    NPC: It seems to me it's been getting colder...
+   [Too warm.]
    NPC: In a way, you could say that.
    To me, it feels it's been much colder than usual.
- Something isn't right.
{ 
    - hadChatWithFriend:
        NPC: Anyway... Good luck with whatever happens out there.
        MC: Thanks...
    - else:
        -> chat ->
}
- -> npcEnd ->
-> END
= chat 
NPC: ...
Would you like to chat for a little bit?
+   [Sure.]
    MC: Sure, why not.
    ~hadChatWithFriend=true
    NPC: Okay.
    ...
    MC: What do you wanna chat about?
    NPC: You've been.. spending a lot of time at the library.
    MC: I like it there.
    NPC: Heard any good one lately?
    MC: Same stuff as usual, mostly.
    NPC: Right.
    NPC: I don't think it's healthy.
    The coralnet exists to release excessive emotions, not for lingering and shutting one's self off.
    MC: It "exists for"? How do you even know that?
    MC: And, besides, It's good for me. It's the only thing I feel like doing anymore. These days.
    NPC: Well...
    At least you're not doing... 
    You know. Whatever everyone's been up to lately?
    MC: The entanglement?
    NPC: Is that what they call it?
    MC: You think there's something wrong with it?
    NPC: Oh? I don't know. I mean, maybe?
    It's just strange. It's not something that we used to do.
    And there's all these people disappearing left and right.. Like your ex-lover. Leaving you high and dry.
    MC: I'm sure it wasn't personal.
    NPC: I wasn't saying it was. 
    MC: Um. I know. I don't know why I said that.
    ~pause(2)
    NPC: Anyway... I don't know.
    It  just doesn't flow right with me. None of it. This shouldn't be the time to "experiment".
    What do YOU think?
    ++  [It's odd.]
        MC: It's odd. Um. The things I've been reading on the coralnet...
        NPC: Yes?
        MC: I don't know. It rubs me the wrong way. But I still don't really know what it is. And it's sort of conflicting. All these organs... 
        NPC: I don't even want to hear about it...
        MC: Well, it's not that bad.
        NPC: It's not?
        MC: I don't know. Still...
    ++  [I don't know.]
        MC: I have no idea, really. I guess I still don't know much about it.
        NPC: Yeah. Neither do I. But what I know is that we were all doing fine before.
        MC: Okay... but...
    ++  [It's fine.]
        MC: It's fine. If people like it... I don't see anything wrong with it.
        NPC: For now.
        MC: We'll have to see I guess.
        And about the surface.. I'm sure those people will be back soon.
        NPC: I'm not so sure.
        MC: Ok...
        ~pause(2)
        But, like...
    -- MC: You're not curious to try it? Not even a little bit? See what it's about?
    NPC: No!! Absolutely not!
    MC: Hahaha, okay.
    NPC: It's not funny. I would never.
    MC: Okay! I'll let you know how it is if I do.
    NPC: Are you serious?
    Is there anyone that...?
    MC: No! No...
    Well...
    NPC: Well?
    MC: There was someone, at the library. I don't know.
    NPC: Yes??
    {   
    - followingTeacher:
        MC: I'm actually going somewhere with them, right now.
        NPC: Oh!
        MC: I don't know. I'm not sure what to expect.
        NPC: Hmm... I see... Well...
    - talkedToTeacherAtDiner:
        MC: They actually want to go with me somewhere.
        NPC: Oh!
        And do you...?
        MC: I don't know. Maybe. I'm not sure what to expect.
        NPC: I see... Well... If you do...
    - else:
        MC: They actually said they'd be at the diner. I was thinking maybe I'd go talk to them again.
        NPC: Oh? And then what?
        MC: I don't know. Maybe something could happen...
        NPC: Hmmm... I see... Well...
        ~changeDesire("Find the library stranger in the diner.")
        ~desireStep=3
    }
    There's probably nothing I can say to dissuade you. But hopefully this will at least quell your curiosity. Regardless..
    Please, don't get eaten? I heard that that's what happens at the end.
    MC: I'll try not to.
    NPC: And stay here. I know it may seem like everyone else is doing it, but we're happy here.
    MC: Not EVERYONE is doing it. It's just a few. And of course I won't. I would never.
    NPC: Good.
+   [I'm busy.]
    MC: Sorry, I have things to do.
- ->->



=== npcInDiner1 ===
# color: 1F7A6E
-> npcStart1 ->
NPC: I like the food here... but it's nothing compared to the buffets at Enkidu's parties!
Have you been to one of those?
+ [Yes.]
    Oh yeah? Me too!
    What was your favorite dish?
    ++ [The jellied morsels.]
        Mmm.. Aah!! I wish the current could take me there here and now!
    ++ [The en-croute noisettes.]
        Mmm.. Aah!! I wish the current could take me there here and now!
    ++ [It's not about the food.]
        MC: You're going to Enkidu's parties for the food? Is that really all you care about?
        NPC: Oh! I mean, uhm, no, of course not! I'm, uuuh, all about the scene!
        The... dancing scene, of course! And the music! I'm with the tides!
        And I really like all that stuff that everyone has been doing lately! 
        I have totally done it too, many times! Do you want to do it with me right now?!
        +++ [Sure.]
            Ah, erm..! Maybe after I finish my meal!
            Or actually, maybe even later than that. 
            I'm supposed to eat you at the end, right? I need an empty stomach! Come back a different time!
            MC: Okay..
        +++ [No thanks.]
            Ah... Well, not for lack of trying!
+ [No.]
    You absolutely have to try it! 
    They will be where most of us are going next, so prepare yourself for the bacchanalia of a lifetime!
-  -> npcEnd ->
-> END

=== npcInDiner2 ===
# color: 2b6136
-> npcStart1 ->
NPC: Have you ever noticed how certain people harmonize differently than others?
NPC: Sometimes I'm unable to connect with someone until I've sung the same note as them, or one that's closer to it.
NPC: What do you think that says about them?
+   [It speaks on their nature.]
    You're right. I think some people are pure-hearted and others naturally evil and this is a really good tell!
+   [It speaks on your relationship.]
    That must be true. Lots of people have evil intentions towards me and this is why they make it so difficult!
+   [It speaks on their strength of character.]
    This has to be right. Some have weaker character and they just cannot bother indulging in beautiful and harmonious compositions!
- -> npcEnd ->
-> END

=== npcInDiner3 ===
# color: 1F7A6E
-> npcStart1 ->
NPC: The food here was much better in the last cycle. Do you recall?
In truth, the further I go in cycles, the better my memory of the food is. Isn't that peculiar?
If I were employed in the kitchen, I would make sure the quality was always maintained to the most delectable standards of yester-cycle, for I have a perfect palate.
Alas, my constitution is too delicate to gaze at fish guts. A shame! \\pauseFor everyone.
- -> npcEnd ->
-> END

=== npcInDiner4 === //Beloo
# color: 1d1c29
-> npcStart1 ->
NPC: Have you heard of the microwave technology? It sounds simply portentous!
Apparently, powerful psychics can focus radiation waves into food to make it warm and thus more delectable!
I envision a wonderful future where we are no longer slaves to the tyranny of current for temperature control!
(Unfortunately, a future that is not for me because I am probably allergic to the microwave.)
(I haven't tried it but it seems likely so I would rather not risk it.)
- -> npcEnd ->
-> END

=== npcInDiner5 ===
# color: 966382
-> npcStart1 ->
NPC: Like clownfish to bioluminescent sea anemone.
-> npcEnd ->
-> END


// TO-DO: Add more random npcs




